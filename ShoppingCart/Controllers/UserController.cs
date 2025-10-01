using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using ShoppingCart.Models.Dto;
using System.Security.Cryptography;
using System.Text;

// JWT
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config; // 注入 IConfiguration

        public UserController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // 註冊新會員
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Request body is required." });

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "Email 已被使用" });

            var user = new User
            {
                UserName = dto.UserName ?? dto.Email,
                Email = dto.Email,
                PasswordHash = ComputeHash(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 產生 JWT token (註冊後回傳 token 與 user info)
            var tokenInfo = GenerateJwtToken(user);

            return Ok(new
            {
                id = user.Id,
                userName = user.UserName,
                email = user.Email,
                token = tokenInfo.tokenString,
                expiresAt = tokenInfo.expiresAt
            });
        }

        // 登入
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Request body is required." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized(new { message = "找不到使用者或憑證錯誤" });

            if (user.PasswordHash != ComputeHash(dto.Password))
                return Unauthorized(new { message = "找不到使用者或憑證錯誤" });

            // 成功：產生 JWT 並回傳
            var tokenInfo = GenerateJwtToken(user);

            return Ok(new
            {
                token = tokenInfo.tokenString,
                expiresAt = tokenInfo.expiresAt,
                user = new { id = user.Id, userName = user.UserName, email = user.Email }
            });
        }

        // 簡單 Hash function
        private string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        // 產生 JWT 的 helper（回傳 token 與到期時間）
        private (string tokenString, DateTime expiresAt) GenerateJwtToken(User user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key");
            var issuer = jwtSection.GetValue<string>("Issuer");
            var audience = jwtSection.GetValue<string>("Audience");
            var expiresInMinutes = jwtSection.GetValue<int>("ExpiresInMinutes");

            var keyBytes = Encoding.UTF8.GetBytes(key ?? "");
            if (keyBytes.Length < 16)
            {
                throw new InvalidOperationException($"JWT Key too short ({keyBytes.Length} bytes). HS256 requires at least 16 bytes. Please set a longer Jwt:Key in appsettings or env var.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("uid", user.Id.ToString())
            };

            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(expiresInMinutes > 0 ? expiresInMinutes : 60);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, expires);
        }
    }
}

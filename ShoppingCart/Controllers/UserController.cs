using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using System.Security.Cryptography;
using System.Text;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // 註冊新會員
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest("Email 已被使用");

            user.PasswordHash = ComputeHash(user.PasswordHash); // 將密碼轉成 Hash
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.UserName, user.Email });
        }

        // 登入
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] User login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null) return NotFound("找不到使用者");

            if (user.PasswordHash != ComputeHash(login.PasswordHash))
                return Unauthorized("密碼錯誤");

            return Ok(new { user.Id, user.UserName, user.Email });
        }

        // 簡單 Hash function
        private string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }
}


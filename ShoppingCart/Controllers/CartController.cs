using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using ShoppingCart.Models.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/Cart/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<CartDto>> GetCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return NotFound(new { message = "Cart not found for this user." });

            var cartDto = ToCartDto(cart);
            return Ok(cartDto);
        }

        // POST /api/Cart/{userId}
        [HttpPost("{userId}")]
        public async Task<IActionResult> AddToCart(int userId, [FromBody] AddToCartDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Request body is required." });
            if (dto.Quantity < 1) return BadRequest(new { message = "Quantity must be >= 1." });

            // 確認 user 存在
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists) return NotFound(new { message = $"User {userId} not found." });

            // 檢查 product 是否存在
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null) return NotFound(new { message = $"Product {dto.ProductId} not found." });

            // 取得或建立 cart
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
            }

            // 合併或新增 CartItem
            var existing = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                cart.Items.Add(newItem);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new { message = "Database update failed.", detail = inner });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Unexpected error.", detail = ex.Message });
            }

            // 取最新 cart（包含 Product）
            var updatedCart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var cartDto = ToCartDto(updatedCart);
            return Ok(cartDto);
        }

        // PUT /api/Cart/{userId}/{cartItemId}  更新數量
        [HttpPut("{userId}/{cartItemId}")]
        public async Task<IActionResult> UpdateItem(int userId, int cartItemId, [FromBody] int quantity)
        {
            if (quantity < 1) return BadRequest(new { message = "Quantity must be >= 1." });

            var cartItem = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == cartItemId && i.Cart.UserId == userId);

            if (cartItem == null) return NotFound(new { message = "CartItem not found." });

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/Cart/{userId}/{cartItemId}
        [HttpDelete("{userId}/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int userId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == cartItemId && i.Cart.UserId == userId);

            if (cartItem == null) return NotFound(new { message = "CartItem not found." });

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DTO
        private CartDto ToCartDto(Cart cart)
        {
            if (cart == null) return null;

            var dto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Product = i.Product == null ? null : new ProductDto
                    {
                        Id = i.Product.Id,
                        Name = i.Product.Name,
                        Description = i.Product.Description,
                        Price = i.Product.Price,
                        StockQuantity = i.Product.StockQuantity,
                        Category = i.Product.Category,
                        ImageUrl = i.Product.ImageUrl
                    }
                }).ToList()
            };

            return dto;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;


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

        // 取得某使用者購物車
        [HttpGet("{userId}")]
        public async Task<ActionResult<Cart>> GetCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return NotFound();
            return cart;
        }

        // 加入商品到購物車
        [HttpPost("{userId}")]
        public async Task<ActionResult<CartItem>> AddToCart(int userId, [FromBody] CartItem item)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
            }

            // 如果商品已存在，累加數量
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            await _context.SaveChangesAsync();
            return Ok(item);
        }

        // 修改購物車商品數量
        [HttpPut("{userId}/{cartItemId}")]
        public async Task<IActionResult> UpdateItem(int userId, int cartItemId, [FromBody] int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == cartItemId && i.Cart.UserId == userId);

            if (cartItem == null) return NotFound();

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 移除購物車商品
        [HttpDelete("{userId}/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int userId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == cartItemId && i.Cart.UserId == userId);

            if (cartItem == null) return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PthAPI.Models;

namespace PthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly DevSmContext _context;

        public CartController(DevSmContext context)
        {
            _context = context;
        }

        // Lấy danh sách giỏ hàng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            return await _context.Carts.ToListAsync();
        }

        // Lấy thông tin giỏ hàng theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound("Sản phẩm trong giỏ hàng không tồn tại.");
            }

            return cart;
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
            if (cart == null || cart.Quantity <= 0 || cart.Price <= 0)
            {
                return BadRequest("Dữ liệu giỏ hàng không hợp lệ.");
            }

            cart.Total = cart.Quantity * cart.Price; // Tính tổng tiền
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCart", new { id = cart.Id }, cart);
        }

        // Cập nhật thông tin sản phẩm trong giỏ hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, Cart cart)
        {
            if (id != cart.Id)
            {
                return BadRequest("ID sản phẩm không hợp lệ.");
            }

            cart.Total = cart.Quantity * cart.Price; // Cập nhật tổng tiền
            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound("Sản phẩm trong giỏ hàng không tồn tại.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound("Sản phẩm trong giỏ hàng không tồn tại.");
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Kiểm tra sản phẩm có tồn tại trong giỏ hàng hay không
        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }

        // Tìm kiếm sản phẩm trong giỏ hàng theo tên
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Cart>>> SearchCarts(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await _context.Carts.ToListAsync();
            }

            var carts = await _context.Carts
                .Where(c => c.Name.Contains(name))
                .ToListAsync();

            return carts;
        }
    }
}

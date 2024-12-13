using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PthAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTongHopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly DevSmContext _context;

        public CategoriesController(DevSmContext context)
        {
            _context = context;
        }

        // Lấy tất cả danh mục
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // Lấy danh mục theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }

            return category;
        }

        // Cập nhật danh mục theo ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest("ID danh mục không hợp lệ.");
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound("Danh mục không tồn tại.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Tạo mới danh mục
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            if (category == null || string.IsNullOrEmpty(category.Title))
            {
                return BadRequest("Tên danh mục không được để trống.");
            }

            category.CreatedDate = DateTime.Now; // Gán ngày tạo tự động
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // Xóa danh mục theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        // Tìm kiếm danh mục theo tên
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Category>>> SearchCategories(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await _context.Categories.ToListAsync(); // Trả về tất cả danh mục nếu không có từ khóa tìm kiếm
            }

            var categories = await _context.Categories
                .Where(c => c.Title.Contains(name)) // Tìm kiếm theo tên
                .ToListAsync();

            return categories;
        }

    }
}

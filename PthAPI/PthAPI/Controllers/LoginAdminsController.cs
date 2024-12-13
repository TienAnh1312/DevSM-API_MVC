using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PthAPI.Models;
using System.Text;
using System.Security.Cryptography;

namespace PthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginAdminsController : ControllerBase
    {
        private readonly DevSmContext _context;

        public LoginAdminsController(DevSmContext context)
        {
            _context = context;
        }

        // API Đăng nhập
        [HttpPost("login")]
        public IActionResult Login([FromBody] Login model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { Message = "Email và mật khẩu không được để trống." });
            }

            // Hash mật khẩu
            var hashedPassword = GetSHA256Hash(model.Password);

            // Kiểm tra thông tin đăng nhập từ bảng AdminUser (tìm tài khoản bất kỳ)
            var user = _context.AdminUsers
                .FirstOrDefault(u => u.Email == model.Email && u.Password == hashedPassword);

            if (user == null)
            {
                return Unauthorized(new { Message = "Email hoặc mật khẩu không đúng." });
            }

            return Ok(new { Message = "Đăng nhập thành công!", Email = user.Email });
        }

        private static string GetSHA256Hash(string input)
        {
            using (var sha256 = new SHA256Managed())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PthAPI.Models;
using System.Security.Cryptography;
using System.Text;


namespace PthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerMemberController : ControllerBase
    {
        private readonly DevSmContext _context;

        public CustomerMemberController(DevSmContext context)
        {
            _context = context;
        }
        // Lấy tất cả người dùng (API)
        [HttpGet("get-all-users")]
        public IActionResult GetAllUsers()
        {
            // Lấy tất cả người dùng chưa bị xóa (Isdelete = 0)
            var users = _context.Customers
                .Where(u => u.Isdelete == 0)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Username,
                    u.Email,
                    u.Phone,
                    u.Address,
                    u.Avatar,
                    u.CreatedDate,
                    u.UpdatedDate
                }).ToList();

            if (users == null || users.Count == 0)
            {
                return NotFound(new { Message = "Không có người dùng nào." });
            }

            return Ok(users);
        }
        // API Đăng nhập
        [HttpPost("login")]
        public IActionResult Login([FromBody] Customer model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { Message = "Email và mật khẩu không được để trống." });
            }

            // Hash mật khẩu
            var hashedPassword = GetSHA256Hash(model.Password);

            // Kiểm tra thông tin đăng nhập từ bảng Customers (tìm tài khoản bất kỳ)
            var user = _context.Customers
                .FirstOrDefault(u => u.Email == model.Email && u.Password == hashedPassword);

            if (user == null)
            {
                return Unauthorized(new { Message = "Email hoặc mật khẩu không đúng." });
            }

            return Ok(new { Message = "Đăng nhập thành công!", Email = user.Email });
        }

        // API Đăng ký
        [HttpPost("register")]
        public IActionResult Register([FromBody] Customer model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { Message = "Email và mật khẩu không được để trống." });
            }

            // Hash mật khẩu
            var hashedPassword = GetSHA256Hash(model.Password);

            // Kiểm tra xem người dùng đã tồn tại chưa
            var existingUser = _context.Customers
                .FirstOrDefault(u => u.Email == model.Email && u.Isdelete == 0);

            if (existingUser != null)
            {
                return Conflict(new { Message = "Email này đã tồn tại." });
            }

            // Nếu người dùng chưa tồn tại, tạo mới tài khoản
            var newUser = new Customer
            {
                Email = model.Email,
                Password = hashedPassword,
                Address = model.Address,
                Phone = model.Phone,
                Username = model.Username,
                Isactive = 1,  // Đánh dấu tài khoản là hoạt động
                Isdelete = 0,  // Đánh dấu tài khoản chưa bị xóa
                CreatedDate = DateTime.Now,
                Name = model.Email // Dùng email làm tên tạm thời
            };

            // Lưu thông tin người dùng mới vào cơ sở dữ liệu
            _context.Customers.Add(newUser);
            _context.SaveChanges();

            return Ok(new { Message = "Đăng ký thành công!", newUser.Email });
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
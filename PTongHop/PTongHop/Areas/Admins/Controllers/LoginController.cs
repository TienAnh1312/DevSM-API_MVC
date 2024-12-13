using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PTongHop.Models;
using System.Text;

namespace PTongHop.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Hiển thị form đăng nhập
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        public async Task<IActionResult> Index(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Chuẩn bị dữ liệu gửi tới API đăng nhập
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:44340/api/LoginAdmins/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // Lưu session khi đăng nhập thành công
                HttpContext.Session.SetString("AdminLogin", model.Email);
                return RedirectToAction("Index", "Dashboard");
            }

            // Xử lý lỗi khi đăng nhập thất bại
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "Đăng nhập thất bại: " + errorMessage);
            return View(model);
        }

        // Đăng xuất
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLogin");
            return RedirectToAction("Index");
        }
    }
}

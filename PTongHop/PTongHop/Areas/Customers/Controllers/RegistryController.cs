using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PTongHop.Model;
using PTongHop.Models;
using System.Text;

namespace PTongHop.Areas.Customers.Controllers
{
    [Area("Customers")]
    public class RegistryController : Controller
    {
        private readonly HttpClient _httpClient;

        public RegistryController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Hiển thị form đăng ký
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Xử lý đăng ký
        [HttpPost]
        public async Task<IActionResult> Index(Customer model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Chuẩn bị dữ liệu gửi tới API đăng ký
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:44340/api/CustomerMember/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // Đăng ký thành công, có thể chuyển đến trang đăng nhập
                return RedirectToAction("Index", "Login");
            }

            // Xử lý lỗi khi đăng ký thất bại
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "Lỗi: " + errorMessage);
            return View(model);
        }
    }
}

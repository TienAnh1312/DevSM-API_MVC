using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PTongHop.Models;
using System.Text;

namespace PTongHop.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // Lấy danh sách giỏ hàng từ API và hiển thị trên View
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Cart");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var carts = JsonConvert.DeserializeObject<List<Cart>>(jsonData);
                ViewBag.total = 0;
                return View(carts); // Trả danh sách giỏ hàng về View
            }
            ViewBag.total = 0;
            return View(new List<Cart>());
        }

        // Gọi API để thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> Add(Cart cart)
        {
            var jsonData = JsonConvert.SerializeObject(cart);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Cart", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // Quay về danh sách giỏ hàng
            }

            ViewBag.ErrorMessage = "Không thể thêm sản phẩm vào giỏ hàng.";
            return View("Error");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public async Task<IActionResult> Remove(int id)
        {
            var response = await _httpClient.DeleteAsync($"Cart/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Không thể xóa sản phẩm khỏi giỏ hàng.";
            return View("Error");
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public async Task<IActionResult> Update(int id, int quantity)
        {
            var cart = new Cart { Id = id, Quantity = quantity };

            var jsonData = JsonConvert.SerializeObject(cart);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Cart/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Không thể cập nhật giỏ hàng.";
            return View("Error");
        }
    }
}

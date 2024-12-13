using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PTongHop.Areas.Admins.Controllers;
using PTongHop.Model;

namespace PTongHop.Areas.Amins.Controllers
{
    //[Area("Admins")]
    public class CategoriesController : BaseController
    {
        private readonly HttpClient _httpClient;

        public CategoriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // Hiển thị tất cả danh mục hoặc danh mục theo từ khóa tìm kiếm
        public async Task<IActionResult> Index(string Title)
        {
            var url = string.IsNullOrEmpty(Title)
                ? "https://localhost:44340/api/Categories" // Nếu không có từ khóa tìm kiếm, trả về tất cả
                : $"https://localhost:44340/api/Categories/search?name={Title}"; // Tìm kiếm theo tên

            var response = await _httpClient.GetStringAsync(url);
            var categories = JsonConvert.DeserializeObject<List<Category>>(response);

            ViewData["SearchTerm"] = Title;  // Lưu giá trị tìm kiếm vào ViewData

            return View(categories);
        }


        // Hiển thị form tạo mới danh mục
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý tạo mới danh mục
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem danh mục đã tồn tại chưa
                var checkCategoryResponse = await _httpClient.GetStringAsync($"https://localhost:44340/api/Categories/search?name={category.Title}");
                var categoriesWithSameName = JsonConvert.DeserializeObject<List<Category>>(checkCategoryResponse);
                var existingCategory = categoriesWithSameName.FirstOrDefault();

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại.");
                    return View(category);
                }

                var response = await _httpClient.PostAsJsonAsync("https://localhost:44340/api/Categories", category);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(category);
        }

        // Hiển thị form chỉnh sửa danh mục
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<Category>($"https://localhost:44340/api/Categories/{id}");
            if (response == null)
            {
                return NotFound();
            }

            return View(response);
        }

        // Xử lý chỉnh sửa danh mục
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                var checkCategoryResponse = await _httpClient.GetStringAsync($"https://localhost:44340/api/Categories/search?name={category.Title}");
                var categoriesWithSameName = JsonConvert.DeserializeObject<List<Category>>(checkCategoryResponse);
                var existingCategory = categoriesWithSameName.FirstOrDefault(c => c.Id != id);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại.");
                    return View(category);
                }

                var response = await _httpClient.PutAsJsonAsync($"https://localhost:44340/api/Categories/{id}", category);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(category);
        }

        // Xóa danh mục
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:44340/api/Categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return BadRequest();
        }
    }
}

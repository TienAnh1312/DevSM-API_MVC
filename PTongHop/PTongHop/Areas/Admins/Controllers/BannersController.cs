using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PTongHop.Areas.Admins.Controllers;
using PTongHop.Model;


namespace PTongHop.Areas.Amins.Controllers
{
    //[Area("Admins")]
    public class BannersController : BaseController
    {
        private readonly HttpClient _httpClient;

        public BannersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // GET: Banners/Index (Hiển thị danh sách Banner)
        public async Task<IActionResult> Index(string keyword = null, int page = 1, int pageSize = 2)
        {
            var response = await _httpClient.GetAsync($"banners/paged/search?keyword={keyword}&page={page}&pageSize={pageSize}");

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var banners = JsonConvert.DeserializeObject<List<Banner>>(jsonContent);

                // Lấy tổng số mục từ header của API
                var totalItems = int.Parse(response.Headers.GetValues("X-Total-Count").FirstOrDefault() ?? "0");

                // Tính tổng số trang
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Truyền dữ liệu đến View
                ViewBag.Keyword = keyword;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(banners);
            }
            else
            {
                // Trường hợp API không trả về thành công
                return View(new List<Banner>());
            }
        }

        // GET: Banners/Create (Hiển thị form tạo Banner mới)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Banners/Create (Xử lý tạo mới Banner)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Banner banner)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("banners", banner);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(banner);
        }

        // GET: Banners/Details/{id} (Trả về chi tiết Banner)
        public async Task<IActionResult> Details(int id)
        {
            var banner = await _httpClient.GetFromJsonAsync<Banner>($"banners/{id}");

            if (banner == null)
            {
                return NotFound();
            }

            return Json(new { name = banner.Title, description = banner.Notes });
        }

        // GET: Banners/Edit/{id} (Hiển thị form chỉnh sửa Banner)
        public async Task<IActionResult> Edit(int id)
        {
            var banner = await _httpClient.GetFromJsonAsync<Banner>($"banners/{id}");
            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // POST: Banners/Edit/{id} (Xử lý chỉnh sửa Banner)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Banner banner)
        {
            if (id != banner.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"banners/{id}", banner);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(banner);
        }

        // GET: Banners/Delete/{id} (Xóa Banner)
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"banners/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return BadRequest();
        }
    }
}

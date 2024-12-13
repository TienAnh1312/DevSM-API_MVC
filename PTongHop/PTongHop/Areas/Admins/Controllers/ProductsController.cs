using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PTongHop.Model;

namespace PTongHop.Areas.Admins.Controllers
{
    //[Area("Admins")]
    public class ProductsController :BaseController
    {
        private readonly HttpClient _httpClient;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        // GET: Admin/Products/Index
        public async Task<IActionResult> Index(Product product)
        {
            var url = string.IsNullOrEmpty(product.Code)
               ? "https://localhost:44340/api/Products" // Nếu không có từ khóa tìm kiếm, trả về tất cả
               : $"https://localhost:44340/api/Products/search?name={product.Code}"; // Tìm kiếm theo tên

            var response = await _httpClient.GetStringAsync(url);

            var products = JsonConvert.DeserializeObject<List<Product>>(response);
            return View(products);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            //// Lấy danh sách các danh mục từ API
            //var response = _httpClient.GetStringAsync("https://localhost:44340/api/Categories").Result;
            //var categories = JsonConvert.DeserializeObject<List<Category>>(response);

            //// Đảm bảo categories không null trước khi gán vào ViewData
            //ViewData["Categories"] = categories ?? new List<Category>();

            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên sản phẩm đã tồn tại chưa
                var checkProductResponse = await _httpClient.GetStringAsync($"https://localhost:44340/api/Products/search?name={product.Code}");
                var productsWithSameName = JsonConvert.DeserializeObject<List<Product>>(checkProductResponse);
                var existingProduct = productsWithSameName.FirstOrDefault(); // Dùng biến này để tránh trùng tên

                if (existingProduct != null)
                {
                    // Nếu tên sản phẩm đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("Name", "Tên sản phẩm này đã tồn tại.");
                    return View(product); // Trả lại view với thông báo lỗi
                }

                // Xử lý hình ảnh nếu có
                var file = Request.Form.Files.FirstOrDefault();
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "products", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.Image = fileName; // Lưu tên file vào sản phẩm
                }

                // Gửi dữ liệu đến API để tạo sản phẩm mới
                var response = await _httpClient.PostAsJsonAsync("https://localhost:44340/api/Products", product);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index"); // Sau khi tạo thành công, chuyển hướng về trang danh sách sản phẩm
                }
            }

            return View(product);
        }


        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Lấy sản phẩm cần sửa
            var response = await _httpClient.GetFromJsonAsync<Product>($"https://localhost:44340/api/Products/{id}");
            if (response == null)
            {
                return NotFound();
            }

            // Lấy danh sách các danh mục
            var categoryResponse = await _httpClient.GetStringAsync("https://localhost:44340/api/Categories");
            var categories = JsonConvert.DeserializeObject<List<Category>>(categoryResponse);
            ViewData["Categories"] = categories;

            return View(response);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Xử lý hình ảnh nếu có
                var file = Request.Form.Files.FirstOrDefault();
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "products", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.Image = fileName; // Lưu tên file vào sản phẩm
                }

                // Gửi dữ liệu đến API để cập nhật sản phẩm
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:44340/api/Products/{id}", product);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:44340/api/Products/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return BadRequest();
        }
    }
}

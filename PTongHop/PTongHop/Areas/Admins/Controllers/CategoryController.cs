using Microsoft.AspNetCore.Mvc;

namespace PTongHop.Areas.Amins.Controllers
{
    [Area("Admins")]
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

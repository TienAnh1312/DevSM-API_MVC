using Microsoft.AspNetCore.Mvc;

namespace PTongHop.Areas.Admins.Controllers
{
    //[Area("Admins")]
    public class DashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

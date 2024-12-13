using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        // GET: api/Dashboard
        [HttpGet]
        public IActionResult Index()
        {

            return Ok();
        }
    }
}

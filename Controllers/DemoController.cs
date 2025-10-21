using Microsoft.AspNetCore.Mvc;

namespace ABAK_NUEVO.Controllers
{
    public class DemoController : Controller
    {
        // GET: /Demo
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace ABAK_NUEVO.Controllers
{
    public class ServiciosController : Controller
    {
        // GET: /Servicios
        public IActionResult Index()
        {
            return View();
        }
    }
}

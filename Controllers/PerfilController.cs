using System.Threading.Tasks;
using ABAK_NUEVO.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ABAK_NUEVO.Controllers
{
    [Authorize] // Solo usuarios logueados pueden ver su perfil
    public class PerfilController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PerfilController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Perfil
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Si por alguna razón no encuentra al usuario, lo mandamos al inicio
                return RedirectToAction("Index", "Home");
            }

            return View(user); // Pasamos el ApplicationUser completo a la vista
        }
    }
}

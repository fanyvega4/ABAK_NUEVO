using System;
using System.Threading.Tasks;
using ABAK_NUEVO.Data;
using ABAK_NUEVO.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ABAK_NUEVO.Controllers
{
    /// <summary>
    /// Sección protegida: Material libre (requiere usuario autenticado).
    /// </summary>
    [Authorize] // requiere login
    public class MaterialLibreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MaterialLibreController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Registra un movimiento en la tabla HistorialAccesos
        /// para la sección "MaterialLibre".
        /// </summary>
        private async Task RegistrarAccesoAsync(string accion)
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty;
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            var log = new HistorialAcceso
            {
                UsuarioId = userId,
                FechaAcceso = DateTime.UtcNow,
                Seccion = "MaterialLibre",
                Accion = accion,
                DireccionIP = ip
            };

            _context.HistorialAccesos.Add(log);
            await _context.SaveChangesAsync();
        }

        // GET: /MaterialLibre
        public async Task<IActionResult> Index()
        {
            await RegistrarAccesoAsync("Acceso");
            return View();
        }
    }
}
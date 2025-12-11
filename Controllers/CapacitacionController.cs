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
    /// Sección protegida: Capacitación (solo usuarios autenticados).
    /// </summary>
    [Authorize] // Requiere usuario logueado
    public class CapacitacionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CapacitacionController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Registra un movimiento en la tabla HistorialAccesos
        /// para la sección "Capacitacion".
        /// </summary>
        private async Task RegistrarAccesoAsync(string accion)
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty;
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            var log = new HistorialAcceso
            {
                UsuarioId = userId,
                FechaAcceso = DateTime.UtcNow,
                Seccion = "Capacitacion",
                Accion = accion,
                DireccionIP = ip
            };

            _context.HistorialAccesos.Add(log);
            await _context.SaveChangesAsync();
        }

        // GET: /Capacitacion
        public async Task<IActionResult> Index()
        {
            await RegistrarAccesoAsync("Acceso");
            return View();
        }
    }
}
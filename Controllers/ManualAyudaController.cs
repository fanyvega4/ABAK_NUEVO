using System;
using System.Linq;                            // Where / OrderBy
using System.Threading.Tasks;
using ABAK_NUEVO.Data;
using ABAK_NUEVO.Models.Identity;
using ABAK_NUEVO.Models.ManualAyuda;          // ArticuloAyuda
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;           // ToListAsync / FirstOrDefaultAsync

namespace ABAK_NUEVO.Controllers
{
    /// <summary>
    /// Sección protegida: Manual de ayuda (solo usuarios autenticados).
    /// </summary>
    [Authorize] // Requiere usuario logueado
    public class ManualAyudaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManualAyudaController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task RegistrarAccesoAsync(string seccion, string accion)
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty;
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            var log = new HistorialAcceso
            {
                UsuarioId = userId,
                FechaAcceso = DateTime.UtcNow,
                Seccion = seccion,
                Accion = accion,
                DireccionIP = ip
            };

            _context.HistorialAccesos.Add(log);
            await _context.SaveChangesAsync();
        }

        // GET: /ManualAyuda
        // Listado de artículos
        public async Task<IActionResult> Index()
        {
            await RegistrarAccesoAsync("ManualAyuda", "Acceso");

            var articulos = await _context.ArticulosAyuda
                .Where(a => a.Activo)
                .OrderByDescending(a => a.FechaPublicacion)
                .ToListAsync();

            return View(articulos);
        }

        // GET: /ManualAyuda/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var articulo = await _context.ArticulosAyuda
                .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

            if (articulo == null)
            {
                return NotFound();
            }

            // Registramos que el usuario vio un artículo específico
            await RegistrarAccesoAsync("ManualAyuda", $"VerArticulo:{id}");

            return View(articulo);
        }
    }
}
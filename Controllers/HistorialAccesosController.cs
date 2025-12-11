using ABAK_NUEVO.Data;
using ABAK_NUEVO.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ABAK_NUEVO.Controllers
{
    [Authorize] // Solo usuarios autenticados pueden ver el historial (y luego validamos admin)
    public class HistorialAccesosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistorialAccesosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --------- HELPERS PARA VALIDAR ADMIN (IGUAL QUE EN AdminController) ---------

        private bool EsAdminActual()
        {
            var email = User?.Identity?.Name?.ToLower() ?? string.Empty;

            // Admin por rol o por correo maestro
            return User.IsInRole("Admin") ||
                   email == "admin@erp-abak.com";
        }

        private IActionResult? VerificarAdmin()
        {
            if (!EsAdminActual())
            {
                // Esto dispara el AccessDenied.cshtml que configuramos
                return Forbid();
            }

            return null;
        }

        // GET: /HistorialAccesos
        [HttpGet]
        public async Task<IActionResult> Index(
            string? email,
            string? accion,
            string? seccion,
            DateTime? desde,
            DateTime? hasta)
        {
            // ⛔ Primero validamos que sea admin
            var authCheck = VerificarAdmin();
            if (authCheck != null) return authCheck;

            // Guardamos filtros en ViewBag para rellenar los campos en la vista
            ViewBag.EmailFiltro = email ?? string.Empty;
            ViewBag.AccionFiltro = accion ?? "Todas";
            ViewBag.SeccionFiltro = seccion ?? "Todas";
            ViewBag.DesdeFiltro = desde?.ToString("yyyy-MM-dd") ?? string.Empty;
            ViewBag.HastaFiltro = hasta?.ToString("yyyy-MM-dd") ?? string.Empty;

            // Base: unimos historial con usuarios para obtener correo y nombre
            var query = from h in _context.HistorialAccesos
                        join u in _context.Users on h.UsuarioId equals u.Id into gj
                        from u in gj.DefaultIfEmpty()
                        select new { h, u };

            // Filtro por correo (contiene)
            if (!string.IsNullOrWhiteSpace(email))
            {
                var texto = email.Trim().ToLower();
                query = query.Where(x => x.u != null &&
                                         x.u.Email.ToLower().Contains(texto));
            }

            // Filtro por acción (Login / Logout / Registro / Acceso, etc.)
            if (!string.IsNullOrWhiteSpace(accion) && accion != "Todas")
            {
                query = query.Where(x => x.h.Accion == accion);
            }

            // Filtro por sección (Login / ManualAyuda / Capacitacion / MaterialLibre / etc.)
            if (!string.IsNullOrWhiteSpace(seccion) && seccion != "Todas")
            {
                query = query.Where(x => x.h.Seccion == seccion);
            }

            // Filtro por fecha desde
            if (desde.HasValue)
            {
                var d = desde.Value.Date;
                query = query.Where(x => x.h.FechaAcceso >= d);
            }

            // Filtro por fecha hasta (incluyendo todo el día)
            if (hasta.HasValue)
            {
                var hFin = hasta.Value.Date.AddDays(1);
                query = query.Where(x => x.h.FechaAcceso < hFin);
            }

            // Ordenamos por fecha descendente y limitamos a 500 registros
            var lista = await query
                .OrderByDescending(x => x.h.FechaAcceso)
                .Take(500)
                .Select(x => new HistorialAccesoVM
                {
                    FechaAcceso = x.h.FechaAcceso,
                    Email = x.u != null ? x.u.Email : "(sin usuario)",
                    Nombre = x.u != null
                        ? $"{x.u.Nombre} {x.u.Apellido}".Trim()
                        : string.Empty,
                    Seccion = x.h.Seccion,
                    Accion = x.h.Accion,
                    DireccionIP = x.h.DireccionIP
                })
                .ToListAsync();

            ViewBag.TotalRegistros = lista.Count;

            return View(lista);
        }
    }
}
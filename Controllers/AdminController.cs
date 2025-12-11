using System;
using System.Linq;
using System.Threading.Tasks;
using ABAK_NUEVO.Data;
using ABAK_NUEVO.Models.Identity;
using ABAK_NUEVO.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ABAK_NUEVO.Controllers
{
    // 🔐 Ahora el controlador completo está restringido al rol Admin
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Helper para decidir si el usuario actual es administrador
        private bool EsAdminActual()
        {
            var email = User?.Identity?.Name?.ToLower() ?? string.Empty;

            return User.IsInRole("Admin") ||
                   email == "admin@erp-abak.com"; // tu admin “maestro”
        }

        private IActionResult? VerificarAdmin()
        {
            if (!EsAdminActual())
            {
                // Puedes cambiar a RedirectToAction si prefieres
                return Forbid();
            }

            return null;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var authCheck = VerificarAdmin();
            if (authCheck != null) return authCheck;

            var hoy = DateTime.UtcNow.Date;

            // Total de usuarios
            var totalUsuarios = await _userManager.Users.CountAsync();

            // Nuevos registros hoy (usamos FechaRegistro de ApplicationUser)
            var nuevosHoy = await _userManager.Users
                .CountAsync(u => u.FechaRegistro.Date == hoy);

            // Para que quede claro en el panel
            var registrosHoy = nuevosHoy;

            // Logins de hoy (HistorialAccesos con Accion = "Login")
            var loginsHoy = await _context.HistorialAccesos
                .Where(h => h.Accion == "Login" && h.FechaAcceso.Date == hoy)
                .CountAsync();

            // Últimos 10 accesos (cualquier acción)
            var ultimos = await _context.HistorialAccesos
                .OrderByDescending(h => h.FechaAcceso)
                .Take(10)
                .Join(
                    _userManager.Users,
                    h => h.UsuarioId,
                    u => u.Id,
                    (h, u) => new UltimoAccesoVM
                    {
                        FechaAcceso = h.FechaAcceso,
                        Email = u.Email,
                        Seccion = h.Seccion,
                        Accion = h.Accion,
                        DireccionIP = h.DireccionIP
                    })
                .ToListAsync();

            var vm = new AdminDashboardVM
            {
                TotalUsuarios = totalUsuarios,
                NuevosHoy = nuevosHoy,
                RegistrosHoy = registrosHoy,
                LoginsHoy = loginsHoy,
                UltimosAccesos = ultimos
            };

            return View(vm);
        }

        // GET: /Admin/Usuarios?pagina=1&tamanoPagina=20
        public async Task<IActionResult> Usuarios(int pagina = 1, int tamanoPagina = 20)
        {
            var authCheck = VerificarAdmin();
            if (authCheck != null) return authCheck;

            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1) tamanoPagina = 20;

            var query = _userManager.Users
                .OrderBy(u => u.Email);

            var total = await query.CountAsync();

            var usuarios = await query
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(u => new UsuarioListadoVM
                {
                    Id = u.Id,
                    Email = u.Email,
                    NombreCompleto = (u.Nombre + " " + u.Apellido).Trim(),
                    Empresa = u.Empresa,
                    Rol = u.Rol,
                    SeccionRegistro = u.SeccionRegistro,
                    FechaRegistro = u.FechaRegistro,
                    UltimoAcceso = u.UltimoAcceso,
                    NumeroContacto = u.NumeroContacto
                })
                .ToListAsync();

            var vm = new AdminUsuariosVM
            {
                Usuarios = usuarios,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = total
            };

            return View(vm);
        }
    }
}

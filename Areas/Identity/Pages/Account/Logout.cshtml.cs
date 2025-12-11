using System;
using System.Threading.Tasks;
using ABAK_NUEVO.Data;
using ABAK_NUEVO.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ABAK_NUEVO.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly ApplicationDbContext _db;

        public LogoutModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LogoutModel> logger,
            ApplicationDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _db = db;
        }

        // Normalmente no se usa GET para logout, pero por si acaso redirigimos a inicio
        public IActionResult OnGet()
        {
            return Redirect("~/");
        }

        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            // Antes de cerrar sesión, registramos la auditoría
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User) ?? string.Empty;
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

                var registro = new HistorialAcceso
                {
                    UsuarioId = userId,
                    FechaAcceso = DateTime.UtcNow,
                    Seccion = "Login",
                    Accion = "Logout",
                    DireccionIP = ip
                };

                _db.HistorialAccesos.Add(registro);
                await _db.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            // Si no viene returnUrl, lo mandamos al Home
            return Redirect("~/");
        }
    }
}
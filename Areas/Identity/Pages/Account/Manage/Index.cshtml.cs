using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ABAK_NUEVO.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ABAK_NUEVO.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        // Para mostrar en la tarjeta derecha
        public DateTime? FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }

        public class InputModel
        {
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Display(Name = "Apellidos")]
            public string Apellidos { get; set; }

            [Display(Name = "Empresa / Área")]
            public string Empresa { get; set; }

            [Phone]
            [Display(Name = "Número de contacto")]
            public string PhoneNumber { get; set; }

            [EmailAddress]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; }

            [Display(Name = "Sección de registro")]
            public string SeccionRegistro { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        private async Task LoadAsync(ApplicationUser user)
        {
            // Cargamos datos del usuario a la vista
            Input = new InputModel
            {
                Nombre = user.Nombre,
                Apellidos = user.Apellido,              // ApplicationUser.Apellido (singular)
                Empresa = user.Empresa,
                PhoneNumber = user.PhoneNumber ?? user.NumeroContacto,
                Email = user.Email,
                SeccionRegistro = user.SeccionRegistro
            };

            FechaRegistro = user.FechaRegistro;
            UltimoAcceso = user.UltimoAcceso;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("No se pudo cargar el usuario actual.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("No se pudo cargar el usuario actual.");
            }

            if (!ModelState.IsValid)
            {
                // Si hay errores de validación, volvemos a mostrar la página
                await LoadAsync(user);
                return Page();
            }

            // Actualizar campos personalizados
            user.Nombre = Input.Nombre?.Trim() ?? string.Empty;
            user.Apellido = Input.Apellidos?.Trim() ?? string.Empty;
            user.Empresa = Input.Empresa?.Trim() ?? string.Empty;
            user.SeccionRegistro = string.IsNullOrWhiteSpace(Input.SeccionRegistro)
                ? "General"
                : Input.SeccionRegistro.Trim();

            // Teléfono / número de contacto
            user.PhoneNumber = Input.PhoneNumber;
            user.NumeroContacto = Input.PhoneNumber;

            // Opcional: refrescar último acceso
            user.UltimoAcceso = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await LoadAsync(user);
                return Page();
            }

            // Refrescamos la cookie de login para que tenga los datos nuevos
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Tu perfil ha sido actualizado correctamente.";

            // Regresamos a la misma página para ver cambios
            return RedirectToPage();
        }
    }
}

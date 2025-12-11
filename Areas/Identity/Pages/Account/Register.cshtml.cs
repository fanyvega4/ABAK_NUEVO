using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ABAK_NUEVO.Data;
using ABAK_NUEVO.Models.Identity;

namespace ABAK_NUEVO.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre es obligatorio.")]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; } = string.Empty;

            [Required(ErrorMessage = "El apellido es obligatorio.")]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; } = string.Empty;

            // 🔹 Alias para coincidir con la vista (asp-for="Input.Apellidos")
            //    Esto permite usar "Apellidos" en el formulario, pero seguir
            //    guardando el valor en la propiedad Apellido que ya usas.
            [Display(Name = "Apellidos")]
            public string Apellidos
            {
                get => Apellido;
                set => Apellido = value;
            }

            [Required(ErrorMessage = "La empresa es obligatoria.")]
            [Display(Name = "Empresa")]
            public string Empresa { get; set; } = string.Empty;

            [Required(ErrorMessage = "El correo es obligatorio.")]
            [EmailAddress(ErrorMessage = "Formato de correo no válido.")]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "La sección es obligatoria.")]
            [Display(Name = "Sección de registro")]
            public string SeccionRegistro { get; set; } = "General";

            [Phone(ErrorMessage = "Formato de teléfono no válido.")]
            [Display(Name = "Número de contacto")]
            public string? NumeroContacto { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null, string? seccion = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!string.IsNullOrEmpty(seccion))
            {
                Input.SeccionRegistro = seccion;
            }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Validación extra: número de contacto obligatorio para Material libre
            if (Input.SeccionRegistro == "MaterialLibre" &&
                string.IsNullOrWhiteSpace(Input.NumeroContacto))
            {
                ModelState.AddModelError("Input.NumeroContacto", "El número de contacto es obligatorio para Material libre.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                Nombre = Input.Nombre,
                Apellido = Input.Apellido,
                Empresa = Input.Empresa,
                SeccionRegistro = Input.SeccionRegistro,
                FechaRegistro = DateTime.UtcNow,
                UltimoAcceso = DateTime.UtcNow,
                Rol = "Usuario",
                NumeroContacto = Input.NumeroContacto
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario creó una nueva cuenta con contraseña.");

                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";

                _context.HistorialAccesos.Add(new HistorialAcceso
                {
                    UsuarioId = user.Id,
                    FechaAcceso = DateTime.UtcNow,
                    Seccion = "Registro",
                    Accion = "Registro",
                    DireccionIP = ip
                });

                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
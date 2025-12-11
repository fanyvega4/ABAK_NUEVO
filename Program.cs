using ABAK_NUEVO.Data;
using ABAK_NUEVO.Models.Identity;   // <-- para ApplicationUser
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

// ----------------------
// Configuración de servicios
// ----------------------
var builder = WebApplication.CreateBuilder(args);

// DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity + Roles (usamos ApplicationUser extendido)
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        // 🔓 En desarrollo NO pedimos confirmación de correo
        options.SignIn.RequireConfirmedAccount = false;

        // 🔐 Opciones básicas de seguridad
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;

        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        // Cada correo debe ser único
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>() // habilita roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 🔐 Cookie de autenticación (tiempo de sesión y rutas estándar)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";

    // 🔸 AQUÍ el cambio importante: mandamos a una vista MVC nuestra
    options.AccessDeniedPath = "/Home/AccessDenied";

    // Sesión expira a los 30 minutos de inactividad
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true; // se renueva mientras el usuario tenga actividad
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ----------------------
// Pipeline HTTP
// ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Archivos estáticos en wwwroot
app.UseStaticFiles();

// EXPONER carpeta "Portada" (fuera de wwwroot) en /Portada
var portadaPath = Path.Combine(app.Environment.ContentRootPath, "Portada");
if (Directory.Exists(portadaPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(portadaPath),
        RequestPath = "/Portada"
    });
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rutas para ÁREAS (Manual, Capacitacion, etc.)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Ruta MVC por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Páginas de Identity (Login/Register)
app.MapRazorPages();

// Crear roles iniciales y un usuario admin si no existen
await SeedRolesAsync(app.Services);

app.Run();

// ----------------------
// Helpers
// ----------------------
static async Task SeedRolesAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // 1. Crear roles "Admin" y "Usuario" si no existen
    string[] roles = { "Admin", "Usuario" };
    foreach (var role in roles)
    {
        if (!await roleMgr.RoleExistsAsync(role))
        {
            await roleMgr.CreateAsync(new IdentityRole(role));
        }
    }

    // 2. Crear usuario administrador por defecto (si no existe)
    var adminEmail = "admin@erp-abak.com";      // 🔁 CAMBIA esto si quieres
    var adminUser = await userMgr.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,               // como no hay correo, lo marcamos confirmado
            Nombre = "Administrador",
            Apellido = "Principal",
            Empresa = "ABAK",
            SeccionRegistro = "Sistema",
            FechaRegistro = DateTime.UtcNow,
            Rol = "Admin"
        };

        // ⚠️ CAMBIA la contraseña por otra que recuerdes
        var createResult = await userMgr.CreateAsync(adminUser, "Admin1");

        if (createResult.Succeeded)
        {
            await userMgr.AddToRoleAsync(adminUser, "Admin");
        }
        // Si hubiera errores, normalmente los registraríamos en log
    }
}

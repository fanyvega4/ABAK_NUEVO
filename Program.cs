using ABAK_NUEVO.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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

// Identity + Roles (usamos IdentityUser por ahora; luego podrás extenderlo)
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        // Si no quieres confirmación por correo en dev:
        // options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>() // habilita roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

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

// Páginas de Identity (Login/Register si usas UI por defecto)
app.MapRazorPages();

// Crear roles iniciales si no existen
await SeedRolesAsync(app.Services);

app.Run();

// ----------------------
// Helpers
// ----------------------
static async Task SeedRolesAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var role in new[] { "Admin", "Usuario" })
    {
        //if (!await roleMgr.RoleExistsAsync(role))
        //    await roleMgr.CreateAsync(new IdentityRole(role));//
    }
}

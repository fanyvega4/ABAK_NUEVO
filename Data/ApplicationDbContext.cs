using ABAK_NUEVO.Models.Identity;
using ABAK_NUEVO.Models.ManualAyuda;          // Para ArticuloAyuda
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ABAK_NUEVO.Data
{
    /// <summary>
    /// DbContext principal del sistema, basado en Identity con ApplicationUser.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tabla de auditoría de accesos
        public DbSet<HistorialAcceso> HistorialAccesos { get; set; } = null!;

        // Tabla de artículos de ayuda
        public DbSet<ArticuloAyuda> ArticulosAyuda { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ----- CONFIGURACIÓN DE ApplicationUser -----
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.Nombre)
                      .HasMaxLength(100);

                entity.Property(u => u.Apellido)
                      .HasMaxLength(100);

                entity.Property(u => u.Empresa)
                      .HasMaxLength(150);

                entity.Property(u => u.SeccionRegistro)
                      .HasMaxLength(50);

                entity.Property(u => u.Rol)
                      .HasMaxLength(30);

                // teléfono / número de contacto (para Material Libre, etc.)
                entity.Property(u => u.NumeroContacto)
                      .HasMaxLength(30);

                // tipos explícitos para fechas
                entity.Property(u => u.FechaRegistro)
                      .HasColumnType("datetime2");

                entity.Property(u => u.UltimoAcceso)
                      .HasColumnType("datetime2");
            });

            // ----- CONFIGURACIÓN DE HistorialAcceso -----
            builder.Entity<HistorialAcceso>(entity =>
            {
                entity.Property(h => h.Seccion)
                      .HasMaxLength(50);

                entity.Property(h => h.Accion)
                      .HasMaxLength(30);

                entity.Property(h => h.DireccionIP)
                      .HasMaxLength(45); // IPv4/IPv6

                entity.Property(h => h.UsuarioId)
                      .HasMaxLength(450); // mismo tamaño que IdentityUser.Id

                entity.Property(h => h.FechaAcceso)
                      .HasColumnType("datetime2");
            });

            // ----- CONFIGURACIÓN DE ArticuloAyuda -----
            builder.Entity<ArticuloAyuda>(entity =>
            {
                entity.Property(a => a.Titulo)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(a => a.Categoria)
                      .HasMaxLength(100);

                entity.Property(a => a.Resumen)
                      .HasMaxLength(500);

                entity.Property(a => a.FechaPublicacion)
                      .HasColumnType("datetime2");

                entity.Property(a => a.ContenidoHtml)
                      .HasColumnType("nvarchar(max)");
            });
        }
    }
}
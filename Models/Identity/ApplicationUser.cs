using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ABAK_NUEVO.Models.Identity
{
    // Usuario de Identity extendido con tus campos extra
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Empresa { get; set; } = string.Empty;

        // "General", "ManualAyuda", "Capacitacion", "MaterialLibre", etc.
        public string SeccionRegistro { get; set; } = "General";

        // SOLO OBLIGATORIO para Material libre (lo validamos en RegisterModel)
        public string? NumeroContacto { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public DateTime UltimoAcceso { get; set; } = DateTime.UtcNow;

        // Solo informativo
        public string Rol { get; set; } = "Usuario";

        // 👉 Propiedad extra para que el código que usa "Apellidos" funcione
        //    No se mapea a la BD, solo envuelve a "Apellido".
        [NotMapped]
        public string Apellidos
        {
            get => Apellido;
            set => Apellido = value;
        }
    }
}

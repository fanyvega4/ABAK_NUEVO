using System;

namespace ABAK_NUEVO.Models.Identity
{
    /// <summary>
    /// Registro de auditoría de accesos (logins / logouts / accesos a secciones).
    /// </summary>
    public class HistorialAcceso
    {
        public int Id { get; set; }

        /// <summary>
        /// Id del usuario de Identity (AspNetUsers.Id)
        /// </summary>
        public string UsuarioId { get; set; } = null!;

        /// <summary>
        /// Momento del acceso.
        /// </summary>
        public DateTime FechaAcceso { get; set; }

        /// <summary>
        /// Sección a la que entra (por ejemplo: "Login", "ManualAyuda", "Capacitacion").
        /// </summary>
        public string Seccion { get; set; } = null!;

        /// <summary>
        /// Tipo de acción (por ejemplo: "Login", "Logout", "Acceso").
        /// </summary>
        public string Accion { get; set; } = null!;

        /// <summary>
        /// Dirección IP del cliente.
        /// </summary>
        public string DireccionIP { get; set; } = null!;
    }
}
using System;

namespace ABAK_NUEVO.Models.Identity
{
    /// <summary>
    /// Modelo de datos para mostrar el historial en la vista.
    /// </summary>
    public class HistorialAccesoVM
    {
        public DateTime FechaAcceso { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string DireccionIP { get; set; } = string.Empty;
    }
}

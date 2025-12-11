using System;

namespace ABAK_NUEVO.Models.Identity
{
    /// <summary>
    /// Fila para mostrar el historial de accesos en una tabla.
    /// </summary>
    public class HistorialAccesoListadoVm
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;

        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }

        public DateTime FechaAccesoLocal { get; set; }

        public string Seccion { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string DireccionIP { get; set; } = string.Empty;
    }
}

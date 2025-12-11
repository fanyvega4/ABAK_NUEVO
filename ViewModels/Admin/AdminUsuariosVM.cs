using System;
using System.Collections.Generic;

namespace ABAK_NUEVO.ViewModels.Admin
{
    public class UsuarioListadoVM
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Empresa { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string SeccionRegistro { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }

        // ⬇⬇⬇ aquí el ajuste: nullable
        public DateTime? UltimoAcceso { get; set; }

        public string NumeroContacto { get; set; } = string.Empty;
    }

    public class AdminUsuariosVM
    {
        public List<UsuarioListadoVM> Usuarios { get; set; } = new();

        public int PaginaActual { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalRegistros { get; set; }

        public int TotalPaginas =>
            (int)Math.Ceiling(TotalRegistros / (double)TamanoPagina);
    }
}

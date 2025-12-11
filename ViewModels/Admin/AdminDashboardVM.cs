using System;
using System.Collections.Generic;

namespace ABAK_NUEVO.ViewModels.Admin
{
    public class UltimoAccesoVM
    {
        public DateTime FechaAcceso { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string DireccionIP { get; set; } = string.Empty;
    }

    public class AdminDashboardVM
    {
        public int TotalUsuarios { get; set; }
        public int NuevosHoy { get; set; }
        public int RegistrosHoy { get; set; }
        public int LoginsHoy { get; set; }

        public List<UltimoAccesoVM> UltimosAccesos { get; set; } = new();
    }
}

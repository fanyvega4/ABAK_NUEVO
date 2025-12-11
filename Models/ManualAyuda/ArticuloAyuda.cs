using System;

namespace ABAK_NUEVO.Models.ManualAyuda
{
    /// <summary>
    /// Artículo del módulo Manual de ayuda.
    /// </summary>
    public class ArticuloAyuda
    {
        public int Id { get; set; }

        /// <summary>
        /// Título principal del artículo.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Categoría (ej. "Facturación", "Producción", "Reportes", etc.).
        /// </summary>
        public string Categoria { get; set; } = string.Empty;

        /// <summary>
        /// Resumen corto que se muestra en el listado.
        /// </summary>
        public string Resumen { get; set; } = string.Empty;

        /// <summary>
        /// Contenido en HTML (se mostrará en la vista Detalle).
        /// </summary>
        public string ContenidoHtml { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de publicación del artículo.
        /// </summary>
        public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Si está activo o no (para poder ocultarlo sin borrarlo).
        /// </summary>
        public bool Activo { get; set; } = true;
    }
}

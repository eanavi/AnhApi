// Archivo: AnhApi.Esquemas/DocumentoEntidadEsq.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    #region CreacionDocumentoEntidad
    /// <summary>
    /// Esquema para la creación de un nuevo documento de entidad.
    /// </summary>
    public class DocumentoEntidadCreacion
    {
        [Required(ErrorMessage = "El ID de la entidad es requerido.")]
        public Guid IdEntidad { get; set; }

        [Required(ErrorMessage = "El tipo de documento de inscripción es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El tipo de documento debe ser un número positivo.")]
        public int TipoDocInscr { get; set; }

        [StringLength(60, ErrorMessage = "El cite no puede exceder los 60 caracteres.")]
        public string? Cite { get; set; }

        [Required(ErrorMessage = "La fecha del documento es requerida.")]
        public DateTime FechaDoc { get; set; } = DateTime.UtcNow; // Asignar fecha actual por defecto

        [StringLength(200, ErrorMessage = "El nombre del archivo no puede exceder los 200 caracteres.")]
        public string? NombreArchivo { get; set; }

        [StringLength(200, ErrorMessage = "La URL del archivo no puede exceder los 200 caracteres.")]
        public string? UrlArchivo { get; set; }

        [StringLength(259, ErrorMessage = "Las observaciones no pueden exceder los 259 caracteres.")]
        public string? Observaciones { get; set; }
    }
    #endregion

    #region DocumentoEntidadListado
    /// <summary>
    /// Esquema para listar un documento de entidad (vista reducida).
    /// </summary>
    public class DocumentoEntidadListado
    {
        [Required(ErrorMessage = "El ID del documento de entidad es requerido.")]
        public int IdDocumentoEntidad { get; set; }

        [Required(ErrorMessage = "El ID de la entidad es requerido.")]
        public Guid IdEntidad { get; set; }

        [Required(ErrorMessage = "El tipo de documento de inscripción es requerido.")]
        public int TipoDocInscr { get; set; }

        public string? Cite { get; set; }
        public DateTime FechaDoc { get; set; }
        public string? NombreArchivo { get; set; }
        public string? UrlArchivo { get; set; }
    }
    #endregion


    #region DocEntidadDespliegue
    /// <summary>
    /// Esquema de despliegue de un documento de entidad (solo informacion de despliegue)
    /// </summary>
    public class DocEntidadDespliegue
    {
        public int IdDocumentoEntidad { get; set; }
        public Guid IdEntidad { get; set; }
        public int TipoDocInscr { get; set; }
        public string? DescripcionTipoDoc { get; set; } = string.Empty;
        public string Cite { get; set; }
        public DateTime FechaDoc { get; set; }
        public string NombreArchivo { get; set; }
        public string UrlArchivo { get; set; }
    }
    #endregion


    #region DocumentoEntidadEsq
    /// <summary>
    /// Esquema completo de un documento de entidad, usado para respuestas detalladas y actualizaciones.
    /// </summary>
    public class EsqDocumentoEntidad : DocumentoEntidadCreacion // Hereda de DocumentoEntidadCreacion
    {
        [Required(ErrorMessage = "El ID del documento de entidad es requerido.")]
        public int IdDocumentoEntidad { get; set; }

        // Campos de auditoría heredados de ModeloBase
        public int? AudEstado { get; set; }
        public string? AudUsuario { get; set; }
        public DateTime? AudFecha { get; set; }
        public string? AudIp { get; set; }
    }
    #endregion
    
}
using System.ComponentModel.DataAnnotations;
using System;
namespace AnhApi.Esquemas
{
    #region CreacionLocalidad
    /// <summary>
    /// Esquema para la creación de una nueva localidad.
    /// </summary>
    public class LocalidadCreacion
    {
        [Required(ErrorMessage = "El ID de la provincia es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la provincia debe ser un número positivo.")]
        public int IdProvincia { get; set; }

        [Required(ErrorMessage = "El nombre de la localidad es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre de la localidad no puede exceder los 100 caracteres.")]
        public string NombreLocalidad { get; set; } = null!;
    }
    #endregion

    #region LocalidadListado
    /// <summary>
    /// Esquema para listar una localidad (vista reducida).
    /// </summary>
    public class LocalidadListado
    {
        [Required(ErrorMessage = "El ID de la localidad es requerido.")]
        public int IdLocalidad { get; set; }

        [Required(ErrorMessage = "El ID de la provincia es requerido.")]
        public int IdProvincia { get; set; }

        [Required(ErrorMessage = "El nombre de la localidad es requerido.")]
        public string NombreLocalidad { get; set; } = null!;
    }
    #endregion

    #region LocalidadEsq
    /// <summary>
    /// Esquema completo de una localidad, usado para respuestas detalladas y actualizaciones.
    /// </summary>
    public class EsqLocalidad : LocalidadCreacion // Hereda de LocalidadCreacion
    {
        [Required(ErrorMessage = "El ID de la localidad es requerido.")]
        public int IdLocalidad { get; set; }

        // Campos de auditoría heredados de ParametroBase
        public int? AudEstado { get; set; }
    }
    #endregion
}
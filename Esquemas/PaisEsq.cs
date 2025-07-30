using System;
using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    #region CreacionPais
    /// <summary>
    /// Esquema para la creación de un nuevo país.
    /// Contiene los campos básicos para registrar un país.
    /// </summary>
    public class PaisCreacion
    {
        [Required(ErrorMessage = "El nombre del país es requerido.")]
        [StringLength(200, ErrorMessage = "El nombre del país no puede exceder los 200 caracteres.")]
        public string NombrePais { get; set; } = null!;

        [Required(ErrorMessage = "La abreviación de 2 caracteres es requerida.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "La abreviación debe tener 2 caracteres.")]
        public string Abreviacion2 { get; set; } = null!;

        [Required(ErrorMessage = "La abreviación de 3 caracteres es requerida.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "La abreviación debe tener 3 caracteres.")]
        public string Abreviacion3 { get; set; } = null!;

        [Required(ErrorMessage = "El código internacional es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El código internacional debe ser un número positivo.")]
        public int CodigoInternacional { get; set; }
    }
    #endregion

    #region PaisListado
    /// <summary>
    /// Esquema para listar un país (vista reducida).
    /// Contiene solo los campos esenciales para un listado.
    /// </summary>
    public class PaisListado
    {
        [Required(ErrorMessage = "El ID del país es requerido.")]
        public int IdPais { get; set; } // Identificador único del país (int4 en DB)

        [Required(ErrorMessage = "El nombre del país es requerido.")]
        public string NombrePais { get; set; } = null!;

        [Required(ErrorMessage = "La abreviación de 2 caracteres es requerida.")]
        public string Abreviacion2 { get; set; } = null!;

        [Required(ErrorMessage = "La abreviación de 3 caracteres es requerida.")]
        public string Abreviacion3 { get; set; } = null!;

        [Required(ErrorMessage = "El código internacional es requerido.")]
        public int CodigoInternacional { get; set; }
    }
    #endregion

    #region PaisEsq
    /// <summary>
    /// Esquema completo de un país, usado para respuestas detalladas y actualizaciones.
    /// Incluye todos los campos de creación y auditoría.
    /// </summary>
    public class PaisEsq : PaisCreacion // Hereda de PaisCreacion para incluir los campos básicos
    {
        [Required(ErrorMessage = "El ID del país es requerido.")]
        public int IdPais { get; set; }

        // Campos de auditoría (asumiendo que los expones en el DTO completo)
        public int? AudEstado { get; set; } // int4 DEFAULT 0 NOT NULL en DB, pero lo hacemos nullable para flexibilidad
    }
    #endregion
}
using System;
using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    #region CreacionDepartamento
    /// <summary>
    /// Esquema para la creación de un nuevo departamento.
    /// Contiene los campos básicos para registrar un departamento.
    /// </summary>
    public class DepartamentoCreacion
    {
        [Required(ErrorMessage = "El ID del país es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del país debe ser un número positivo.")]
        public int IdPais { get; set; }

        [Required(ErrorMessage = "El ID de la región geográfica es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la región geográfica debe ser un número positivo.")]
        public int IdRegionGeografica { get; set; }

        [Required(ErrorMessage = "El nombre del departamento es requerido.")]
        [StringLength(50, ErrorMessage = "El nombre del departamento no puede exceder los 50 caracteres.")]
        public string NombreDepartamento { get; set; } = null!;

        [Required(ErrorMessage = "La abreviación internacional es requerida.")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "La abreviación internacional debe tener 1 caracter.")]
        public string AbrevInt { get; set; } = null!;

        [Required(ErrorMessage = "La abreviatura de 2 caracteres es requerida.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "La abreviatura de 2 caracteres debe tener 2 caracteres.")]
        public string Abrev2 { get; set; } = null!;

        [Required(ErrorMessage = "La abreviatura de 3 caracteres es requerida.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "La abreviatura de 3 caracteres debe tener 3 caracteres.")]
        public string Abrev3 { get; set; } = null!;
    }
    #endregion

    #region DepartamentoListado
    /// <summary>
    /// Esquema para listar un departamento (vista reducida).
    /// Contiene solo los campos esenciales para un listado.
    /// </summary>
    public class DepartamentoListado
    {
        [Required(ErrorMessage = "El ID del departamento es requerido.")]
        public int IdDepartamento { get; set; }

        [Required(ErrorMessage = "El ID del país es requerido.")]
        public int IdPais { get; set; }

        [Required(ErrorMessage = "El nombre del departamento es requerido.")]
        public string NombreDepartamento { get; set; } = null!;

        [Required(ErrorMessage = "La abreviación internacional es requerida.")]
        public string AbrevInt { get; set; } = null!;

        [Required(ErrorMessage = "La abreviatura de 2 caracteres es requerida.")]
        public string Abrev2 { get; set; } = null!;

        [Required(ErrorMessage = "La abreviatura de 3 caracteres es requerida.")]
        public string Abrev3 { get; set; } = null!;
    }
    #endregion

    #region DepartamentoEsq
    /// <summary>
    /// Esquema completo de un departamento, usado para respuestas detalladas y actualizaciones.
    /// Incluye todos los campos de creación y auditoría.
    /// </summary>
    public class EsqDepartamento : DepartamentoCreacion // Hereda de DepartamentoCreacion para incluir los campos básicos
    {
        [Required(ErrorMessage = "El ID del departamento es requerido.")]
        public int IdDepartamento { get; set; }

        // Campos de auditoría (asumiendo que los expones en el DTO completo)
        public int? AudEstado { get; set; } // int4 DEFAULT 0 NOT NULL en DB, pero lo hacemos nullable para flexibilidad

    }

    public class DeptoConProvinciasEsq: EsqDepartamento
    {
        [Required(ErrorMessage = "La lista de provincias no puede ser nula")]
        public ICollection<ProvinciaListado> Provincias { get; set; } = new List<ProvinciaListado>();
    }
    #endregion
}
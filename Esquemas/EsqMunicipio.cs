using System;
using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    #region CreacionMunicipio
    /// <summary>
    /// Esquema para la creación de un nuevo municipio.
    /// </summary>
    public class MunicipioCreacion
    {
        [Required(ErrorMessage = "El ID de la provincia es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la provincia debe ser un número positivo.")]
        public int IdProvincia { get; set; }

        [Required(ErrorMessage = "El nombre del municipio es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre del municipio no puede exceder los 100 caracteres.")]
        public string NombreMunicipio { get; set; } = null!;

        [Required(ErrorMessage = "La sección municipal es requerida.")]
        [StringLength(50, ErrorMessage = "La sección no puede exceder los 50 caracteres.")]
        public string Seccion { get; set; } = null!;
    }
    #endregion

    #region MunicipioListado
    /// <summary>
    /// Esquema para listar un municipio (vista reducida).
    /// </summary>
    public class MunicipioListado
    {
        [Required(ErrorMessage = "El ID del municipio es requerido.")]
        public int IdMunicipio { get; set; }

        [Required(ErrorMessage = "El ID de la provincia es requerido.")]
        public int IdProvincia { get; set; }

        [Required(ErrorMessage = "El nombre del municipio es requerido.")]
        public string NombreMunicipio { get; set; } = null!;

        [Required(ErrorMessage = "La sección municipal es requerida.")]
        public string Seccion { get; set; } = null!;
    }
    #endregion

    #region MunicipioEsq
    /// <summary>
    /// Esquema completo de un municipio, usado para respuestas detalladas y actualizaciones.
    /// </summary>
    public class EsqMunicipio : MunicipioCreacion // Hereda de MunicipioCreacion
    {
        [Required(ErrorMessage = "El ID del municipio es requerido.")]
        public int IdMunicipio { get; set; }

        // Campos de auditoría heredados de ModeloBase
        public int? AudEstado { get; set; }
    }

    public class EstructuraMunicipio
    {
        public int IdNivel { get; set; }
        public string Detalle { get; set; }
        public string Nivel { get; set; }
    }
    #endregion
}
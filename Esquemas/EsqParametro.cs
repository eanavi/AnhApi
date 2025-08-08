using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{

    /// <summary>
    /// Son los campos necesarios para crear un nuevo parámetro.
    /// </summary>
    public class ParametroCreacion
    {
        [Required]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "La descripción es requerida.")]
        [StringLength(120, ErrorMessage = "La descripción no puede exceder los 120 caracteres.")]
        public string Descripcion { get; set; } = null!;

        [StringLength(10, ErrorMessage = "La sigla no puede exceder los 10 caracteres.")]
        public string? Sigla { get; set; }

        [Required(ErrorMessage = "El grupo es requerido.")]
        [StringLength(60, ErrorMessage = "El grupo no puede exceder los 60 caracteres.")]
        public string Grupo { get; set; } = null!;

    }


    public class ParametroCmbLit
    {
        [Required]
        public string Sigla { get; set; }

        [Required]
        [StringLength(120)]
        public string Descripcion { get; set; }
    }

    public class ParametroCmb
    {
        [Required]
        public int Codigo { get; set; }

        [Required]
        [StringLength(120)]
        public string Descripcion { get; set; }
    }

    /// <summary>
    /// Son los campos necesarios para listar un Parametro
    /// </summary>
    public class ParametroListado : ParametroCreacion
    {
        [Required(ErrorMessage = "El ID del parámetro es requerido.")]
        public int IdParametro { get; set; }

    }



    /// <summary>
    /// Son los totales de un archivo
    /// </summary>
    public class EsqParametro : ParametroListado
    {
        public int AudEstado { get; set; } = 0; // Estado por defecto
    }
}

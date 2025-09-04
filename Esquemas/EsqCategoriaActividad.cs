using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    public class CategoriaActividadCreacion
    {
        public int? IdPadreCategoriaActividad { get; set; }

        [Required(ErrorMessage = "El Organigrama es requerido.")]
        public int IdOrganigrama { get; set; }

        [Required(ErrorMessage = "El Nombre de la Categoría es requerido.")]
        public string NombreCategoriaActividad { get; set; }

    }

    public class CategoriaActividadListado
    {
        [Required(ErrorMessage = "El identificador de categoria actividad es obligatorio ")]
        public int IdCategoriaActividad { get; set; }
        public string? CategoriaPadre { get; set; }
        [Required(ErrorMessage = "El nombre de la Categoria es obligatorio")]
        public string NombreCategoriaPadre { get; set; }
    }

    public class EsqCategoriaActividad : CategoriaActividadCreacion
    {
        public int? IdCategoriaActividad { get; set; }
        // Campos de auditoría
        public int? AudEstado { get; set; }

        [Required(ErrorMessage = "El usuario de auditoría es requerido.")]
        [StringLength(30, ErrorMessage = "El usuario de auditoría no puede exceder los 30 caracteres.")]
        public string AudUsuario { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de auditoría es requerida.")]
        public DateTime AudFecha { get; set; }

        [Required(ErrorMessage = "La IP de auditoría es requerida.")]
        [StringLength(15, ErrorMessage = "La IP de auditoría no puede exceder los 15 caracteres.")]
        public string AudIp { get; set; } = null!;
    }
}

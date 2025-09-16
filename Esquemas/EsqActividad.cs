using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    public class ActividadCreacion
    {
        [Required(ErrorMessage = "La categoría de actividad es requerida.")]
        public int IdCategoriaActividad { get; set; }

        [Required(ErrorMessage = "La operación es requerida.")]
        public int IdOperacion { get; set; }

        [Required(ErrorMessage = "El organigrama es requerido.")]
        public int IdOrganigrama { get; set; }

        [Required(ErrorMessage = "El nombre de la actividad es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre de la actividad no puede exceder los 100 caracteres.")]
        public string NombreActividad { get; set; }

        public int? TipoLicencia { get; set; }

        [StringLength(20, ErrorMessage = "El mnemónico no puede exceder los 20 caracteres.")]
        public string? Mnemonico { get; set; }
    }

    public class ActividadListado
    {
        public int IdActividad { get; set; }
        public string NombreActividad { get; set; }
        public string? Mnemonico { get; set; }
        public string? NombreCategoriaActividad { get; set; }
        public string? NombreOrganigrama { get; set; }
    }

    public class EsqActividad : ActividadCreacion
    {
        public int IdActividad { get; set; }

        // Campos de auditoría
        public int AudEstado { get; set; }

        [Required(ErrorMessage = "El usuario de auditoría es requerido.")]
        [StringLength(30, ErrorMessage = "El usuario de auditoría no puede exceder los 30 caracteres.")]
        public string AudUsuario { get; set; } = null;

        [Required(ErrorMessage = "La fecha de auditoría es requerida.")]
        public DateTime AudFecha { get; set; }

        [Required(ErrorMessage = "La IP de auditoría es requerida.")]
        [StringLength(15, ErrorMessage = "La IP de auditoría no puede exceder los 15 caracteres.")]
        public string AudIp { get; set; } = null;
    }
}
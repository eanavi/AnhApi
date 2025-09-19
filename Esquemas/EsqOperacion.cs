using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnhApi.Esquemas
{
    public class OperacionCreacion
    {
        [Required(ErrorMessage = "El nombre de la operación es requerido.")]
        [StringLength(80, ErrorMessage = "El nombre de la operación no puede exceder los 80 caracteres.")]
        [Column("nombre_operacion")]
        public string NombreOperacion { get; set; } = null!;

        [Required(ErrorMessage = "El prefijo es requerido.")]
        [StringLength(20, ErrorMessage = "El prefijo no puede exceder los 10 caracteres.")]
        [Column("prefijo")]
        public string Prefijo { get; set; } = null!;

        [Required(ErrorMessage = "El prefijo_nombre es requerido.")]
        [StringLength(90, ErrorMessage = "El prefijo_nombre no puede exceder los 90 caracteres.")]
        [Column("prefijo_nombre")]
        public string PrefijoNombre { get; set; } = null!;

        [Required(ErrorMessage = "El campo 'desde' es requerido.")]
        [Column("desde")]
        public int Desde { get; set; }

        [Column("hasta")]
        public int? Hasta { get; set; }
        [Column("id_anterior")]
        public int? IdAnterior { get; set; }

        [Column("id_operacion_padre")]
        public int? IdOperacionPadre { get; set; }

        [Column("id_correlativo_config")]
        public int? IdCorrelativoConfig { get; set; }

        [Column("nivel")]
        public int? Nivel { get; set; }
    }

    public class OperacionListado
    {
        [Column("id_operacion")]
        public int IdOperacion { get; set; }
        [Column("nombre_operacion")]
        public string NombreOperacion { get; set; } = null!;
        [Column("prefijo")]
        public string Prefijo { get; set; } = null!;
        [Column("prefijo_nombre")]
        public string PrefijoNombre { get; set; } = null!;
        [Column("desde")]
        public int Desde { get; set; }
        [Column("hasta")]
        public int? Hasta { get; set; }
        [Column("id_anterior")]
        public int? IdAnterior { get; set; }
        [Column("id_operacion_padre")]
        public int? IdOperacionPadre { get; set; }
        [Column("id_correlativo_config")]
        public int? IdCorrelativoConfig { get; set; }
        [Column("nivel")]
        public int? Nivel { get; set; }
    }
    public class EsqOperacion : OperacionCreacion
    {
        [Key]
        [Column("id_operacion")]
        public int IdOperacion { get; set; }

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

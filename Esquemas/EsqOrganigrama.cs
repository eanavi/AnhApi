using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnhApi.Esquemas
{
    public class OrganigramaCreacion
    {
        [Column("id_organigrama_padre")]
        public int? IdOrganigramaPadre { get; set; }
        [Required(ErrorMessage = "El departamento es requerido.")]
        [Column("id_departamento")]
        public int IdDepartamento { get; set; }
        [Column("id_sirh")]
        public int? IdSirh { get; set; }
        [Required(ErrorMessage = "El nombre del organigrama es requerido.")]
        [StringLength(250, ErrorMessage = "El nombre del organigrama no puede exceder los 250 caracteres.")]
        [Column("nombre_organigrama")]
        public string NombreOrganigrama { get; set; } = null!;
        [Required(ErrorMessage = "La sigla es requerida.")]
        [StringLength(50, ErrorMessage = "La sigla no puede exceder los 50 caracteres.")]
        [Column("sigla")]
        public string Sigla { get; set; }
        [Column("nivel")]
        public int? Nivel { get; set; }
        [Column("tipo_organigrama")]
        public int? TipoOrganigrama { get; set; }
        [Column("clase_organigrama")]
        public int? ClaseOrganigrama { get; set; }
    }

    public class OrganigramaListado
    {
        [Column("id_organigrama")]
        public int IdOrganigrama { get; set; }
        [Column("id_organigrama_padre")]
        public int? IdOrganigramaPadre { get; set; }
        [Column("id_departamento")]
        public int IdDepartamento { get; set; }
        [Column("id_sirh")]
        public int? IdSirh { get; set; }
        [Column("nombre_organigrama")]
        public string NombreOrganigrama { get; set; }
        [Column("sigla")]
        public string Sigla { get; set; }
        [Column("nivel")]
        public int? Nivel { get; set; }
        [Column("tipo_organigrama")]
        public int? TipoOrganigrama { get; set; }
        [Column("clase_organigrama")]
        public int? ClaseOrganigrama { get; set; }
    }

    public class OrganigramaNodoDto
    {
        public int IdOrganigrama { get; set; }
        public string NombreOrganigrama { get; set; } = null!;
        public string Sigla { get; set; } = null!;

        // Hijos (dependientes)
        public List<OrganigramaNodoDto> Hijos { get; set; } = new();
    }


    public class EsqOrganigrama : OrganigramaCreacion
    {
        [Key]
        [Column("id_organigrama")]
        public int IdOrganigrama { get; set; }

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

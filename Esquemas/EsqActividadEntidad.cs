using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnhApi.Esquemas
{
    public class ActividadEntidadCreacion
    {
        [Required(ErrorMessage = "El ID de la entidad es requerido.")]
        public Guid Id_Entidad { get; set; }
        [Required(ErrorMessage = "El ID de la actividad es requerido.")]
        public int Id_Actividad { get; set; }
    }

    public class ActividadEntidadListado
    {
        [Column("id_actividad_entidad")]
        public int Id_Actividad_Entidad { get; set; }
        [Column("id_entidad")]
        public Guid Id_Entidad { get; set; }
        [Column("entidad")]
        public string Entidad { get; set; }
        [Column("actividad")]
        public string Actividad { get; set; }
    }

    public class EsqActividadEntidad : ActividadEntidadCreacion
    {
        [Required(ErrorMessage = "El ID de la relación es requerido.")]
        public int Id_Actividad_Entidad { get; set; }
    }
}

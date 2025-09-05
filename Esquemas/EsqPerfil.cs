using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    public class PerfilCreacion
    {
        [Required(ErrorMessage = "La descripción del Perfil es requerida")]
        [StringLength(100, ErrorMessage = "La descripción no puede exceder los 100 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El área del Perfil es requerida")]
        [StringLength(50, ErrorMessage = "El área no puede exceder los 50 caracteres.")]
        public string Area { get; set; }

        [Required(ErrorMessage = "El Id de la Unidad es requerido")]
        public int Id_Unidad { get; set; }
    }

    public class PerfilListado
    {
        [Required(ErrorMessage = "El Id del Perfil es requerido")]
        public int IdPerfil { get; set; }

        [Required(ErrorMessage = "La descripción del Perfil es requerida")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El área del Perfil es requerida")]
        public string Area { get; set; }

        [Required(ErrorMessage = "El Id de la Unidad es requerido")]
        public int Id_Unidad { get; set; }

    }

    public class EsqPerfil : PerfilCreacion
    {
        [Required(ErrorMessage = "El Id del Perfil es requerido")]
        public int IdPerfil { get; set; }
    }
}
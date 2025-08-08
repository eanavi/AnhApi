using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    public class ProvinciaCreacion
    {
        [Required(ErrorMessage = "El ID del departamento es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del departamento debe ser un número positivo.")]
        public int IdDepartamento { get; set; }
        [Required(ErrorMessage = "El id auxiliar de la provincia es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El id auxiliar de la provincia debe ser un número positivo.")]
        public int IdProvinciaAux { get; set; }
        [Required(ErrorMessage = "El nombre de la provincia es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre de la provincia no puede exceder los 100 caracteres.")]
        public string NombreProvincia { get; set; } = null!;
    }

    public class ProvinciaListado
    {
        [Required(ErrorMessage = "El ID de la provincia es requerido.")]
        public int IdProvincia { get; set; }

        [Required(ErrorMessage = "El ID auxiliar de la provincia es requerido.")]
        public int IdProvinciaAux { get; set; }
        
        [Required(ErrorMessage = "El ID del departamento es requerido.")]
        public int IdDepartamento { get; set; }
        [Required(ErrorMessage = "El nombre de la provincia es requerido.")]
        public string NombreProvincia { get; set; } = null!;
    }

    public class EsqProvincia : ProvinciaCreacion
    {
        [Required(ErrorMessage = "El ID de la provincia es requerido.")]
        public int IdProvincia { get; set; }
        public int? AudEstado { get; set; }
    }

    public class ProvConMunicipiosEsq : EsqProvincia 
    {
        [Required(ErrorMessage = "La lista de municipios no puede ser nula")]
        public ICollection<MunicipioListado> Municipios { get; set; } = new List<MunicipioListado>();
    }

    public class ProvConLocalidadesEsq: EsqProvincia
    {
        [Required(ErrorMessage = "La Lista de localidades no puede ser nula")]
        public ICollection<LocalidadListado> Localidades { get; set; } = new List<LocalidadListado>();
    }


}

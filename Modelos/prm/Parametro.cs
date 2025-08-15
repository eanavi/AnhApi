using AnhApi.Interfaces;

namespace AnhApi.Modelos.prm
{
    public class Parametro : ParametroBase //Ojo no hereda de Modelo Base, solo tiene el campo de estado
    {
        public int id_parametro { get; set; } // int4 y serial4
        public int codigo { get; set; } // int4 NOT NULL
        public string descripcion { get; set; } = null!; // varchar(120) NOT NULL
        public string? sigla { get; set; } // varchar(10) NULL
        public string grupo { get; set; } = null!; // varchar(60) NOT NULL

        public ICollection<RepresentanteEntidad> RepresentantesEntidadParametro { get; set; } = new List<RepresentanteEntidad>(); // Relación con RepresentanteEntidadParametro
    }
}

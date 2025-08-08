using System;

namespace AnhApi.Modelos.prm
{
    public class Pais : ParametroBase
    {
        // Codigo de pais ISO 3166-1 alfa-2 (int4 en la DB)
        public int id_pais { get; set; }

        // Nombre del Pais (varchar(200) NOT NULL)
        public string nombre_pais { get; set; } = null!;

        // Codigo de pais iso 3166-1 alfa-2 (varchar(2) NOT NULL)
        public string abreviacion2 { get; set; } = null!;

        // Codigo de pais iso 3166-1 alfa-3 (varchar(3) NOT NULL)
        public string abreviacion3 { get; set; } = null!;

        // Codigo de pais iso 3166-1 alfa-numeric (int4 NOT NULL)
        public int codigo_internacional { get; set; }

        public ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
    }
}
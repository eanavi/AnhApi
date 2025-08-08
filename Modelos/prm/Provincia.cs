namespace AnhApi.Modelos.prm
{
    public class Provincia : ParametroBase
    {
        public int id_provincia { get; set; } // Clave primaria: id_provincia (serial4 en DB, mapea a int)
        public int id_provincia_aux { get; set; } // Clave auxiliar: id_provincia_aux (int4 en DB, mapea a int)
        public int id_departamento { get; set; } // Clave foránea: id_departamento (int4 NOT NULL en DB)
        public string nombre_provincia { get; set; } = null!; // Campo: nombre_provincia (varchar(50) NOT NULL en DB)
        public Departamento Departamento { get; set; } = null!;

        public ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
        public ICollection<Localidad> Localidades { get; set; } = new List<Localidad>();
    }
}
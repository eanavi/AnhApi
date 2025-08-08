using AnhApi.Modelos.prm;
using System;
using System.Collections.Generic; // Necesario para ICollection si la usaras aquí
using AnhApi.Interfaces; // Para IAuditable, si ModeloBase lo implementa

namespace AnhApi.Modelos.prm
{
    public class Localidad : ParametroBase
    {
        // Clave primaria: id_localidad (serial4 en DB, mapea a int)
        public int id_localidad { get; set; }

        // Clave foránea: id_provincia (int4 NOT NULL en DB)
        // Comentario: "Provincia a la que pertenece"
        public int id_provincia { get; set; }

        // Nombre de la Localidad (varchar(100) NOT NULL)
        public string nombre_localidad { get; set; } = null!;

        // Propiedad de navegación para la relación con Provincia
        // Una Localidad pertenece a una Provincia (id_provincia NOT NULL)
        public Provincia Provincia { get; set; } = null!;
    }
}

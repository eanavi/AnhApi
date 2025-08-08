using System;
using System.Collections.Generic;

namespace AnhApi.Modelos.prm
{
    // Asumimos que Provincia es un modelo válido en el mismo namespace
    // public class Provincia : ModeloBase { ... }

    // Hereda de ModeloBase para los campos de auditoría (aud_estado, aud_usuario, aud_ip, aud_fecha)
    public class Municipio : ParametroBase
    {
        // Clave primaria: id_municipio (serial4 en DB, mapea a int)
        public int id_municipio { get; set; }

        // Clave foránea: id_provincia (int4 NOT NULL en DB)
        public int id_provincia { get; set; }

        // Nombre del Municipio (varchar(100) NOT NULL)
        public string nombre_municipio { get; set; } = null!;

        // Seccion municipal (varchar(50) NOT NULL)
        public string seccion { get; set; } = null!;

        // Propiedad de navegación para la relación con Provincia
        // Un Municipio pertenece a una Provincia (id_provincia NOT NULL)
        public Provincia Provincia { get; set; } = null!;
    }
}
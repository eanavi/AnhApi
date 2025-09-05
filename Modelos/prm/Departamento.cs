using System;
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AnhApi.Modelos.prm;

namespace AnhApi.Modelos
{
    // Hereda de ModeloBase para los campos de auditoría (aud_estado, aud_usuario, aud_ip, aud_fecha)
    public class Departamento : ParametroBase
    {
        // Clave primaria: id_departamento (serial4 en DB, mapea a int)
        public int id_departamento { get; set; }

        // Clave foránea: id_pais (int4 NOT NULL en DB)
        public int id_pais { get; set; }

        // Campo: id_region_geografica (int4 NOT NULL en DB)
        // Comentario: "Parametro region geografica"
        public int id_region_geografica { get; set; }

        // Campo: nombre_departamento (varchar(50) NOT NULL en DB)
        // Comentario: "Nombre del departamento"
        public string nombre_departamento { get; set; } = null!;

        // Campo: abrev_int (bpchar(1) NOT NULL en DB)
        // Comentario: "Abreviacion internaciona iso 3166-2"
        public string abrev_int { get; set; } = null!;

        // Campo: abrev_2 (varchar(2) NOT NULL en DB)
        // Comentario: "Abreviatura de dos caracteres"
        public string abrev_2 { get; set; } = null!;

        // Campo: abrev_3 (varchar(3) NOT NULL en DB)
        // Comentario: "Abreviatura de tres caracteres"
        public string abrev_3 { get; set; } = null!;

        // Propiedad de navegación para la relación con Pais
        // Un Departamento pertenece a un Pais (id_pais NOT NULL)
        // La propiedad es no-nullable porque la FK es NOT NULL en la base de datos.
        public Pais Pais { get; set; } = null!;

        public ICollection<Provincia> Provincias { get; set; } = new List<Provincia>();
    }
}
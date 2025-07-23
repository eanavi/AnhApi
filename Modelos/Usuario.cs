// Archivo: AnhApi.Modelos/Usuario.cs
using System;
using AnhApi.Interfaces; // Para IAuditable, ya que ModeloBase lo implementa
using System.ComponentModel.DataAnnotations.Schema; // Para [DatabaseGenerated] si lo usas en el modelo

namespace AnhApi.Modelos
{
    // Hereda de ModeloBase para los campos de auditoría
    public class Usuario : ModeloBase
    {

        public long id_usuario { get; set; }

        public Guid id_persona { get; set; }

        public int id_perfil { get; set; }

        public string login { get; set; } = null!; // varchar(50) NOT NULL

        public string clave { get; set; } = null!; // varchar(100) NOT NULL (almacenará el hash de la contraseña)

        public Persona Persona { get; set; } = null!; // Relación con Persona, asumiendo que tienes una clase Persona definida

    }
}
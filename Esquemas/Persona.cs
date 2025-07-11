using System;
using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    public class Persona
    {
        [Required]
        public Guid IdPersona { get; set; }

        [Required]
        [StringLength(60)]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(30)]
        public string PrimerApellido { get; set; } = null!;

        [Required]
        [StringLength(30)]
        public string SegundoApellido { get; set; } = null!;

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(20)]
        public string NumeroIdentificacion { get; set; } = null!;

        [StringLength(10)]
        public string? Complemento { get; set; }
        public object? Direccion { get; set; } // JSONB como string para la API
        public object? Telefono { get; set; }  // JSONB como string para la API
        public object? Correo { get; set; }    // JSONB como string para la API
        public int? AudEstado { get; set; }

    }
}

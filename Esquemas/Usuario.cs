using System;
using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    public class Usuario
    {
        [Required]
        public long IdUsuario { get; set; }

        [Required]
        public Guid IdPersona { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Clave { get; set; } = null!;

    }
}
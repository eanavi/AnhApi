using System;
using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    public class UsuarioCreacion
    {
        [Required]
        public Guid IdPersona { get; set; }

        [Required]
        public int IdPerfil { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Clave { get; set; } = null!;
    }

    public class UsuarioListado
    {
        [Required]
        public long IdUsuario { get; set; }

        [Required]
        public Guid IdPersona { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; } = null!;

    }

    public class EsqUsuario : UsuarioListado
    {
        [Required]
        [StringLength(100)]
        public string Clave { get; set; } = null!;
    }
}
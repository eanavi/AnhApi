using System.ComponentModel.DataAnnotations;

namespace AnhApi.Modelos
{
    public class Usuario : ModeloBase
    {
        public long id_usuario { get; set; } // numeric(10) mapeado a long
        public Guid id_persona { get; set; }
        public string login { get; set; } = null!;
        public string clave { get; set; } = null!;
    }
}

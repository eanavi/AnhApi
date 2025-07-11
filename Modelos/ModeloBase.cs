using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnhApi.Modelos
{
    public abstract class ModeloBase
    {
        public int aud_estado { get; set; } = 0;

        public string aud_usuario { get; set; }
        public string aud_ip{ get; set; }
        public DateTime aud_fecha { get; set; } = DateTime.UtcNow;
    }
}


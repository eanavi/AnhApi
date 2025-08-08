using Microsoft.EntityFrameworkCore;

namespace AnhApi.Modelos.prm
{
    [Keyless]
    public class Jerarquia
    {
        public int id { get; set; }
        public string detalle{ get; set; }
        public string nivel { get; set; }
    }
}

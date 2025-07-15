using System.Text.Json;
namespace AnhApi.Modelos
{
    public class Persona : ModeloBase
    {

        public Guid id_persona { get; set; }
        public string nombre { get; set; } = null!;
        public string primer_apellido { get; set; } = null!;
        public string segundo_apellido { get; set; } = null!;
        public DateTime fecha_nacimiento { get; set; }
        public string numero_identificacion { get; set; } = null!;
        public string? complemento { get; set; }
        public int genero { get; set; } = 0; // -1: No especificado, 1: Masculino, 0: Femenino
        public JsonDocument? direccion { get; set; }
        public JsonDocument? telefono { get; set; }
        public JsonDocument? correo { get; set; }
    }
}


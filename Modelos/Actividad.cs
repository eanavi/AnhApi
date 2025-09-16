namespace AnhApi.Modelos
{
    public class Actividad : ModeloBase
    {
        public int id_actividad { get; set; }
        public int id_categoria_actividad { get; set; }
        public int id_operacion { get; set; }
        public int id_organigrama { get; set; }
        public string nombre_actividad { get; set; }
        public int? tipo_licencia { get; set; }
        public string? mnemonico { get; set; }

        // Navigation properties
        public CategoriaActividad? CategoriaActividad { get; set; }
        public Organigrama? Organigrama { get; set; }
    }
}
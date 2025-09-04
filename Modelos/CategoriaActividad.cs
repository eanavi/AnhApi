namespace AnhApi.Modelos
{
    public class CategoriaActividad : ModeloBase
    {
        public int id_categoria_actividad { get; set; }
        public int? id_padre_categoria_actividad { get; set; }
        public int id_organigrama { get; set; }
        public string nombre_categoria_actividad { get; set; }

        public CategoriaActividad? CategoriaPadre { get; set; }

        public ICollection<CategoriaActividad> Hijos { get; set; } = new List<CategoriaActividad>();

    }
}

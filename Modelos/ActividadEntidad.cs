namespace AnhApi.Modelos
{
    public class ActividadEntidad : ModeloBase
    {
        public int id_actividad_entidad { get; set; }
        public int id_entidad { get; set; }
        public int id_actividad { get; set; }

        public Entidad Entidad { get; set; } = null!;

        public Actividad Actividad { get; set; } = null!;
    }
}

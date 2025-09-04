namespace AnhApi.Modelos
{
    public class Organigrama : ModeloBase
    {
        public int id_organigrama { get; set; }
        public int? id_organigrama_padre { get; set; }
        public int id_departamento { get; set; }
        public int? id_sirh { get; set; }
        public string nombre_organigrama { get; set; }
        public string sigla { get; set; }
        public int? nivel { get; set; }
        public int? tipo_organigrama { get; set; }
        public int? clase_organigrama { get; set; }
        public Organigrama? OrganigramaPadre { get; set; }
        public ICollection<Organigrama>? Dependientes { get; set; } = new List<Organigrama>();
    }
}
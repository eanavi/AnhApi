namespace AnhApi.Modelos
{
    public class Operacion : ModeloBase
    {
        public int id_operacion { get; set; }
        public string nombre_operacion { get; set; }
        public string prefijo { get; set; }
        public string prefijo_nombre { get; set; }
        public DateTime desde { get; set; }
        public DateTime? hasta { get; set; }
        public int? id_anterior { get; set; }
        public int? id_operacion_padre { get; set; }
        public int? id_correlativo_config { get; set; }
        public int? nivel { get; set; }
    }
}

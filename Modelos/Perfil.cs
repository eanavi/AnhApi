namespace AnhApi.Modelos
{
    public class Perfil : ModeloBase
    {
        public int IdPerfil { get; set; }
        public string Descripcion { get; set; }
        public string Area { get; set; }
        public int Id_Unidad { get; set; }
    }
}

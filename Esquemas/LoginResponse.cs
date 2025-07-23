namespace AnhApi.Esquemas
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Usuario { get; set; }
        public string Rol { get; set; }
        public DateTime FechaExpiracion { get; set; }
    }
}

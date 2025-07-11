namespace AnhApi.Nucleo
{
    public class BdPostgres
    {
        public string Host { get; set; } = string.Empty;
        public int Puerto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;

        public string GetConnectionString()
        {
            return $"Host={Host};Port={Puerto};Database={Nombre};Username={Usuario};Password={Clave};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=30;Connection Idle Lifetime=60";
        }
    }
}

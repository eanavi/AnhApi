namespace AnhApi.Interfaces
{
    public interface IAuditable
    {
        int aud_estado { get; set; }
        string aud_usuario { get; set; }
        string aud_ip { get; set; }
        DateTime aud_fecha { get; set; }

    }
}

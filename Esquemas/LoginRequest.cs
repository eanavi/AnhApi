using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required(ErrorMessage = "El campo 'login' es obligatorio.")]
    [StringLength(50, ErrorMessage = "El campo 'login' no puede exceder los 50 caracteres.")]
    public string Login { get; set; }
    [Required(ErrorMessage = "El campo 'clave' es obligatorio.")]
    [StringLength(100, ErrorMessage = "El campo 'clave' no puede exceder los 100 caracteres.")]
    public string Clave { get; set; }
}
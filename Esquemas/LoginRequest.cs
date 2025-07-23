using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required(ErrorMessage = "El campo 'Usuario' es obligatorio.")]
    [StringLength(50, ErrorMessage = "El campo 'Usuario' no puede exceder los 50 caracteres.")]
    public string Login { get; set; }
    [Required(ErrorMessage = "El campo 'Contraseña' es obligatorio.")]
    [StringLength(100, ErrorMessage = "El campo 'Contraseña' no puede exceder los 100 caracteres.")]
    public string Clave { get; set; }
}
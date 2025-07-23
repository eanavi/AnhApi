using System.Threading.Tasks;
using AnhApi.Esquemas;

namespace AnhApi.Servicios
{
    public interface IAuthServicio
    {
        /// <summary>
        /// Intenta autenticar a un usuario y, si tiene éxito, genera un token JWT.
        /// </summary>
        /// <param name="request">Contiene el login y la clave del usuario.</param>
        /// <returns>Un LoginResponse con el token y datos del usuario, o null si la autenticación falla.</returns>
        Task<LoginResponse?> AutenticarUsuario(LoginRequest request);

    }
}

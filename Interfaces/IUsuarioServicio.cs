using AnhApi.Modelos;
using AnhApi.Esquemas;
namespace AnhApi.Interfaces
{
    public interface IUsuarioServicio : IGenericoServicio<Usuario, long>
    {
        Task<UsuarioEsq> CrearUsuario(UsuarioCreacion usuarioCreacion, string usuario, string ip);
        Task<UsuarioEsq?> ObtenerPorLogin(string login);
    }
}

using AnhApi.Esquemas;
using AnhApi.Modelos;

namespace AnhApi.Interfaces
{
    public interface IServicioUsuario : IServicioAuditoria<Usuario, long>
    {
        Task<EsqUsuario> CrearUsuario(UsuarioCreacion usuarioCreacion, string usuario, string ip);
        Task<EsqUsuario?> ObtenerPorLogin(string login);
    }
}

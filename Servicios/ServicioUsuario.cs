using AnhApi.Datos;
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class ServicioUsuario : ServicioAuditoria<Usuario, long>, IServicioUsuario
    {
        private readonly IMapper _mapper;
        private readonly ContextoAppBD _contexto;
        private readonly ILogger<ServicioUsuario> _logger;

        public ServicioUsuario(ContextoAppBD contexto, ILogger<ServicioUsuario> logger, IMapper mapper) : base(contexto, logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<EsqUsuario> CrearUsuario(UsuarioCreacion usuarioCreacion, string usuario, string ip)
        {
            try
            {
                var usr = _mapper.Map<Usuario>(usuarioCreacion);

                usr.clave = BCrypt.Net.BCrypt.HashPassword(usuarioCreacion.Clave, 12);
                usr.aud_usuario = usuario;
                usr.aud_ip = ip;

                _contexto.Usuarios.Add(usr);
                await _contexto.SaveChangesAsync();

                return _mapper.Map<EsqUsuario>(usr);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario.");
                throw; // Puedes lanzar una excepción personalizada si lo deseas

            }
        }

        public async Task<EsqUsuario?> ObtenerPorLogin(string login)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login))
                {
                    throw new ArgumentException("El login no puede ser nulo o vacío.", nameof(login));
                }
                var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.login == login);
                if (usuario == null)
                {
                    return null;
                }
                return _mapper.Map<EsqUsuario>(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario por login.");
                throw; // Puedes lanzar una excepción personalizada si lo deseas
            }
        }
    }
}

using AnhApi.Datos;
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class ServicioPerfil : ServicioAuditoria<Perfil, int>, IServicioPerfil
    {
        private readonly ContextoAppBD _contexto;
        private readonly ILogger<ServicioPerfil> _logger;
        private readonly IMapper _mapper;

        public ServicioPerfil(ContextoAppBD contexto, ILogger<ServicioPerfil> logger, IMapper mapper)
            : base(contexto, logger) // Llama al constructor de GenericoServicio
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    }
}

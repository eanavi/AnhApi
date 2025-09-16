using AnhApi.Datos;
using AnhApi.Esquemas;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AnhApi.Servicios
{
    public class ServicioActividad : ServicioAuditoria<Actividad, int>, Interfaces.IservicioActividad
    {
        private readonly Datos.ContextoAppBD _contexto;
        private readonly ILogger<ServicioActividad> _logger;
        private readonly IMapper _mapper;

        public ServicioActividad(Datos.ContextoAppBD contexto, ILogger<ServicioActividad> logger, IMapper mapper) : base(contexto, logger)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(ContextoAppBD));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }
    }
}
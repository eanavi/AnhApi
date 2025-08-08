using AnhApi.Modelos;
using AnhApi.Datos;
using AutoMapper;
using AnhApi.Interfaces;

namespace AnhApi.Servicios
{
    public class ServicioDocumentoEntidad : ServicioAuditoria<DocumentoEntidad, int>, IServicioDocumentoEntidad
    {
        private readonly ContextoAppBD _contextoBd;
        private readonly ILogger<ServicioDocumentoEntidad> _logger;
        private readonly IMapper _mapper;
        public ServicioDocumentoEntidad(ContextoAppBD contextoBd, ILogger<ServicioDocumentoEntidad> logger, IMapper mapper)
            : base(contextoBd, logger)
        {
            _contextoBd = contextoBd ?? throw new ArgumentNullException(nameof(contextoBd));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper;
        }
    }
}

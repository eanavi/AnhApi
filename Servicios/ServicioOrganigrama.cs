using AnhApi.Modelos;
using AnhApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class ServicioOrganigrama : ServicioAuditoria<Organigrama, int>, IServicioOrganigrama
    {
        private readonly Datos.ContextoAppBD _contexto;
        private readonly ILogger<ServicioOrganigrama> _logger;
        public ServicioOrganigrama(
            Datos.ContextoAppBD contexto,
            ILogger<ServicioOrganigrama> logger) : base(contexto, logger)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}

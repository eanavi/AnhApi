using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class ServicioCategoriaActividad : ServicioAuditoria<CategoriaActividad, int>, IServicioCategoriaActividad
    {
        private readonly Datos.ContextoAppBD _contexto;
        private readonly ILogger<ServicioCategoriaActividad> _logger;
        private readonly IMapper _mapper;

        public ServicioCategoriaActividad(
            Datos.ContextoAppBD contexto,
            ILogger<ServicioCategoriaActividad> logger,
            IMapper mapper) : base(contexto, logger)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<CategoriaActividadListado>> ListadoCategoria()
        {
            try
            {
                var categorias = await _contexto.CategoriasActividad
                    .Where(p => p.aud_estado == 0)
                    .Include(c => c.CategoriaPadre)
                    .ProjectTo<CategoriaActividadListado>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return categorias;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de Categorias ");
                throw;
            }
        }

    }
}

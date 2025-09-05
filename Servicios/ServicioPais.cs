using AnhApi.Datos;
using AnhApi.Interfaces;
using AnhApi.Modelos.prm;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class ServicioPais : ServicioGenerico<Pais, int>, IServicioPais
    {
        private readonly ContextoAppBD _contextoBd;
        private readonly ILogger<ServicioPais> _logger;
        private readonly IMapper _mapper;

        public ServicioPais(ContextoAppBD contextoBd, ILogger<ServicioPais> logger, IMapper mapper)
            : base(contextoBd, logger)
        {
            _contextoBd = contextoBd ?? throw new ArgumentNullException(nameof(contextoBd));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Pais>> Buscar(string criterio)
        {
            try
            {
                var query = _contextoBd.Set<Pais>().AsQueryable();

                if (string.IsNullOrWhiteSpace(criterio))
                {
                    query = _contextoBd.Set<Pais>().Where(p => p.aud_estado == 0);
                }
                else
                {
                    query = _contextoBd.Set<Pais>()
                        .FromSqlRaw("SELECT * FROM \"public\".\"buscar_pais\"(@criterio)",
                        new Npgsql.NpgsqlParameter("criterio", criterio));
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar países con criterio: {Criterio}", criterio);
                throw; // Vuelve a lanzar la excepción después de registrar el error
            }

        }

        public async Task<Pais?> ObtenerPaisConDepartamentosAsync(int id)
        {
            try
            {
                var pais = await _contextoBd.Paises
                            .Include(p => p.Departamentos)
                            .FirstOrDefaultAsync(p => p.id_pais == id && p.aud_estado == 0);
                if (pais == null)
                {
                    _logger.LogWarning($"Pais con id {id} no encontrado");
                }

                return pais;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Pais con id {id}", id);
                throw;
            }
        }

    }
}

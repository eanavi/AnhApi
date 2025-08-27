using AnhApi.Datos;
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AnhApi.Modelos.prm;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AnhApi.Servicios
{
    public class ServicioProvincia : ServicioGenerico<Provincia, int>, IServicioProvincia
    {
        private readonly ContextoAppBD _contextoBd;
        private readonly ILogger<ServicioProvincia> _logger;
        private readonly IMapper _mapper;
        public ServicioProvincia(ContextoAppBD contextoBd, ILogger<ServicioProvincia> logger, IMapper mapper) : base(contextoBd, logger)
        {
            _contextoBd = contextoBd ?? throw new ArgumentNullException(nameof(contextoBd));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Buscar provincias
        /// </summary>
        /// <param name="criterio"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Provincia>> Buscar(string criterio)
        {
            try
            {
                var query = _contextoBd.Set<Provincia>().AsQueryable();
                if (string.IsNullOrWhiteSpace(criterio))
                {
                    query = _contextoBd.Set<Provincia>().Where(p => p.aud_estado == 0);
                }
                else
                {
                    query = _contextoBd.Set<Provincia>()
                        .FromSqlRaw("SELECT * FROM \"public\".\"buscar_provincia\"(@criterio)",
                        new Npgsql.NpgsqlParameter("criterio", criterio));
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar provincias con criterio: {Criterio}", criterio);
                throw; // Vuelve a lanzar la excepción después de registrar el error
            }
        }

        public async Task<Provincia?> ObtenerProvinciaConMunicipiosAsync(int id)
        {
            try
            {
                var prov = await _contextoBd.Provincias
                    .Include(p => p.Municipios)
                    .FirstOrDefaultAsync(p => p.id_provincia == id && p.aud_estado == 0);

                if (prov == null)
                {
                    _logger.LogWarning($"Provincia con id {id} no enonctrada");
                }

                return prov;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la provincia con id {id}", id);
                throw;
            }
        }

        public async Task<Provincia?> ObtenerProvinciaConLocalidadesAsync(int id)
        {
            try
            {
                var prov = await _contextoBd.Provincias
                    .Include(l => l.Localidades)
                    .FirstOrDefaultAsync(p => p.id_provincia == id && p.aud_estado == 0);

                if (prov == null)
                {
                    _logger.LogWarning($"Provinica con id {id} no encontrada");
                }

                return prov;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la provincia con id {id}", id);
                throw;
            }
        }

    }
}

using AnhApi.Datos;
using AnhApi.Interfaces;
using AnhApi.Modelos.prm;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class ServicioLocalidad : ServicioGenerico<Localidad, int>, IServicioLocalidad
    {
        private readonly ContextoAppBD _contextoBd;
        private readonly ILogger<ServicioLocalidad> _logger;
        private readonly IMapper _mapper;

        public ServicioLocalidad(ContextoAppBD contextoBd, ILogger<ServicioLocalidad> logger, IMapper mapper) : base(contextoBd, logger)
        {
            _contextoBd = contextoBd ?? throw new ArgumentNullException(nameof(contextoBd));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        async Task<IEnumerable<Localidad>> IServicioLocalidad.Buscar(string criterio)
        {
            try
            {
                var query = _contextoBd.Set<Localidad>().AsQueryable();
                if (string.IsNullOrWhiteSpace(criterio))
                {
                    query = _contextoBd.Set<Localidad>().Where(p => p.aud_estado == 0);
                }
                else
                {
                    query = _contextoBd.Set<Localidad>()
                        .FromSqlRaw("Select * from \"public\".\"buscar_localidad\"(@criterio)",
                        new Npgsql.NpgsqlParameter("criterio", criterio));
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar Localidaes con criterio {criterio}", criterio);
                throw;//Vuelve a lanzar la excpecion
            }
        }

        async Task<IEnumerable<Jerarquia>> IServicioLocalidad.BuscarJerarquia(string criterio)
        {
            try
            {
                var query = _contextoBd.Set<Jerarquia>().AsQueryable();
                if (!string.IsNullOrWhiteSpace(criterio))
                {
                    query = _contextoBd.Set<Jerarquia>()
                        .FromSqlRaw("Select * from \"public\".\"estructura_localidad\"(@criterio)",
                        new Npgsql.NpgsqlParameter("criterio", criterio));
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar jerarquia de localidades con criterio {criterio}", criterio);
                throw;//Vuelve a lanzar la excpecion
            }
        }
    }
}

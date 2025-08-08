using Microsoft.EntityFrameworkCore;
using AnhApi.Datos;
using AnhApi.Modelos.prm;
using AutoMapper;
using AnhApi.Esquemas;
using AnhApi.Interfaces;


namespace AnhApi.Servicios
{
    public class ServicioMunicipio : ServicioGenerico<Municipio, int>, IServicioMunicipio
    {
        private readonly ContextoAppBD _contextoBd;
        private readonly ILogger<ServicioMunicipio> _logger;
        private readonly IMapper _mapper;

        public ServicioMunicipio(ContextoAppBD contextoBd, ILogger<ServicioMunicipio> logger, IMapper mapper) : base (contextoBd, logger)
        {
            _contextoBd = contextoBd ?? throw new ArgumentNullException(nameof(contextoBd));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Municipio>> Buscar(string criterio)
        {
            try
            {
                var query = _contextoBd.Set<Municipio>().AsQueryable();
                if (string.IsNullOrWhiteSpace(criterio))
                {
                    query = _contextoBd.Set<Municipio>().Where(p => p.aud_estado == 0);
                } else
                {
                    query = _contextoBd.Set<Municipio>()
                        .FromSqlRaw("Select * from \"public\".\"buscar_municipio\"(@criterio)", 
                        new Npgsql.NpgsqlParameter("criterio", criterio));
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar municipios con criterio {criterio}", criterio);
                throw;//Vuelve a lanzar la excpecion
            }
        }

        public async Task<IEnumerable<Jerarquia>> BuscarJerarquia(string criterio)
        {
            try
            {
                var query = _contextoBd.Set<Jerarquia>().AsQueryable();
                if (!string.IsNullOrWhiteSpace(criterio))
                {
                    query = _contextoBd.Set<Jerarquia>()
                        .FromSqlRaw("Select * from \"public\".\"estructura_municipio\"(@criterio)",
                        new Npgsql.NpgsqlParameter("criterio", criterio));
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar municipios con criterio {criterio}", criterio);
                throw;//Vuelve a lanzar la excpecion
            }
        }

    }
}

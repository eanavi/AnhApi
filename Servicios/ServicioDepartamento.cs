using AnhApi.Datos;
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AnhApi.Modelos.prm;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class ServicioDepartamento : ServicioGenerico<Departamento, int>, IServicioDepartamento
    {
        private readonly ContextoAppBD _contextoBd;
        private readonly ILogger<ServicioDepartamento> _logger;
        private readonly IMapper _mapper;

        public ServicioDepartamento(ContextoAppBD contextoBd, ILogger<ServicioDepartamento> logger, IMapper mapper) : base(contextoBd, logger)
        {
            _contextoBd = contextoBd ?? throw new ArgumentNullException(nameof(contextoBd));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Departamento>> Buscar(string criterio)
        {
            try
            {
                var query = _contextoBd.Set<Departamento>().AsQueryable();

                if (string.IsNullOrWhiteSpace(criterio))
                {
                    query = _contextoBd.Set<Departamento>().Where(p => p.aud_estado == 0);
                }
                else
                {
                    query = _contextoBd.Set<Departamento>()
                        .FromSqlRaw("SELECT * FROM \"public\".\"buscar_departamento\"(@criterio)",
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

        public async Task<Departamento?> ObtenerDeptoConProvinciasAsync(int id)
        {
            try
            {
                var depto = await _contextoBd.Departamentos
                    .Include(p => p.Provincias)
                    .FirstOrDefaultAsync(p => p.id_departamento == id && p.aud_estado == 0);

                if (depto == null)
                {
                    _logger.LogWarning($"Departamento con id {id} no encontrado");
                }

                return depto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el departamento con id {id}", id);
                throw;
            }
        }

    }
}

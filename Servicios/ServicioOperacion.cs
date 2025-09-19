using AnhApi.Interfaces;
using AnhApi.Modelos;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace AnhApi.Servicios
{
    public class ServicioOperacion : ServicioAuditoria<Operacion, int>, IServicioOperacion
    {
        private readonly Datos.ContextoAppBD _contexto;
        private readonly ILogger<ServicioOperacion> _logger;
        public ServicioOperacion(
            Datos.ContextoAppBD contexto,
            ILogger<ServicioOperacion> logger) : base(contexto, logger)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Operacion>> Buscar(string criterio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criterio))
                {
                    return await ObtenerTodosAsync();
                }

                var operaciones = await _contexto.Operaciones
                                .FromSqlRaw("select * from \"public\".\"buscar_operacion\"(@crierio", 
                                    new NpgsqlParameter("crierio", criterio))
                                .ToListAsync();
                return operaciones;
            }
            catch(NpgsqlException ex)
            {
                _logger.LogError(ex, $"Error de Postgrsql al buscar con criterio '{criterio}'");
                throw new ApplicationException($"Error al ejecutar el procedimiento de busqueda de operaciones ", ex);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error inesperado al buscar operaciones");
                throw;
            }
        }
    }
}

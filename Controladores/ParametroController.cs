// Archivo: AnhApi.Api.Controladores/ParametroController.cs
using System.Security.Claims; // Para obtener información del usuario autenticado
using AnhApi.Esquemas; // Para los DTOs PaisCreacion, PaisListado, PaisEsq, PaginacionResultado, PaginacionParametros
using AnhApi.Interfaces; // Para IParametroServicio, ILogger
using AnhApi.Modelos.prm; // Para el modelo Parametro
using AnhApi.Servicios; // Para ParametroServicio
using AutoMapper; // Para el mapeo entre modelos y DTOs
using Microsoft.AspNetCore.Mvc; // Para [ApiController], [Route], ActionResult, etc.
using Microsoft.Extensions.Logging; // Para ILogger

namespace AnhApi.Api.Controladores
{
    [ApiController]
    [Route("api/[controller]")] // La ruta base será /api/Parametro
    // [Authorize] // Descomenta si este controlador requiere autenticación
    public class ParametroController : ControllerBase
    {
        private readonly IServicioParametro _parametroServicio;
        private readonly IMapper _mapper;
        private readonly ILogger<ParametroController> _logger;

        public ParametroController(
            IServicioParametro parametroServicio,
            IMapper mapper,
            ILogger<ParametroController> logger)
        {
            _parametroServicio = parametroServicio ?? throw new ArgumentNullException(nameof(parametroServicio));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene una lista paginada de parámetros activos.
        /// </summary>
        /// <param name="parametrosPaginacion">Parámetros para la paginación (número de página, tamaño de página).</param>
        /// <returns>Un objeto paginado que contiene una lista de ParametroListado.</returns>
        [HttpGet] // GET /api/Parametro
        [ProducesResponseType(typeof(PaginacionResultado<PaisListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginacionResultado<PaisListado>>> ObtenerParametrosPaginados(
            [FromQuery] PaginacionParametros parametrosPaginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Parámetros de paginación inválidos.");
                    return BadRequest(ModelState);
                }

                var resultadoPaginadoModelos = await _parametroServicio.ObtenerTodosPagAsync(parametrosPaginacion);

                // Mapear los elementos del resultado paginado al DTO de listado
                var parametrosListado = _mapper.Map<IEnumerable<ParametroListado>>(resultadoPaginadoModelos.Elementos);

                var resultadoDto = new PaginacionResultado<ParametroListado>
                {
                    Elementos = parametrosListado,
                    TotalRegistros = resultadoPaginadoModelos.TotalRegistros,
                    PaginaActual = resultadoPaginadoModelos.PaginaActual,
                    TamanoPagina = resultadoPaginadoModelos.TamanoPagina,
                    TotalPaginas = resultadoPaginadoModelos.TotalPaginas
                };

                return Ok(resultadoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener parámetros paginados.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor al obtener parámetros.");
            }
        }


        [HttpGet("grupo/{grupo}")]
        [ProducesResponseType(typeof(IEnumerable<ParametroCmb>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ParametroCmb>>> ObtenerGrupo([FromRoute] string grupo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var grupoResultado = await _parametroServicio.ObtenerPorGrupo(grupo ?? "");
                var lista = _mapper.Map<IEnumerable<ParametroCmb>>(grupoResultado);
                return Ok(lista);
            }
            catch (Exception es)
            {
                _logger.LogError(es, $"Error al obtener el grupo {grupo}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor al obtener el grupo {grupo}");
            }
        }


        /// <summary>
        /// Obtiene un parámetro por su ID.
        /// </summary>
        /// <param name="id">El ID del parámetro.</param>
        /// <returns>Un objeto ParametroEsq si se encuentra, o NotFound si no.</returns>
        [HttpGet("{id}")] // GET /api/Parametro/{id}
        [ProducesResponseType(typeof(EsqPais), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqPais>> ObtenerParametroPorId([FromRoute] int id)
        {
            try
            {
                var parametro = await _parametroServicio.ObtenerPorIdAsync(id);
                if (parametro == null)
                {
                    _logger.LogWarning($"Parámetro con ID {id} no encontrado.");
                    return NotFound($"Parámetro con ID {id} no encontrado.");
                }
                var parametroEsq = _mapper.Map<EsqPais>(parametro);
                return Ok(parametroEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el parámetro con ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor al obtener el parámetro.");
            }
        }

        /// <summary>
        /// Crea un nuevo parámetro.
        /// </summary>
        /// <param name="parametroCreacionDto">Datos del parámetro a crear.</param>
        /// <returns>El parámetro creado con su ID y campos de auditoría.</returns>
        [HttpPost] // POST /api/Parametro
        [ProducesResponseType(typeof(EsqPais), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqPais>> CrearParametro([FromBody] PaisCreacion parametroCreacionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Datos de creación de parámetro inválidos.");
                    return BadRequest(ModelState);
                }

                var parametroModel = _mapper.Map<Parametro>(parametroCreacionDto);

                // Asignar campos de auditoría (ejemplo, ajusta según tu lógica de obtención de usuario/IP)
                // string usuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "sistema";
                // string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "N/A";
                string usuario = "test_user"; // Reemplaza con lógica real de autenticación/obtención de usuario
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"; // Reemplaza con lógica real de obtención de IP

                var nuevoParametro = await _parametroServicio.CrearAsync(parametroModel);
                var parametroEsq = _mapper.Map<EsqPais>(nuevoParametro);

                return CreatedAtAction(
                    nameof(ObtenerParametroPorId),
                    new { id = parametroEsq.IdPais },
                    parametroEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el parámetro.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor al crear el parámetro.");
            }
        }

        /// <summary>
        /// Actualiza un parámetro existente.
        /// </summary>
        /// <param name="id">El ID del parámetro a actualizar.</param>
        /// <param name="parametroEsqDto">Datos actualizados del parámetro.</param>
        /// <returns>NoContent si la actualización fue exitosa, o NotFound/BadRequest.</returns>
        [HttpPut("{id}")] // PUT /api/Parametro/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarParametro([FromRoute] int id, [FromBody] EsqPais parametroEsqDto)
        {
            try
            {
                if (id != parametroEsqDto.IdPais)
                {
                    _logger.LogWarning($"Conflicto de ID: el ID de la ruta ({id}) no coincide con el ID del cuerpo ({parametroEsqDto.IdPais}).");
                    return BadRequest("El ID de la ruta no coincide con el ID del cuerpo de la solicitud.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Datos de actualización de parámetro inválidos.");
                    return BadRequest(ModelState);
                }

                var parametroModel = _mapper.Map<Parametro>(parametroEsqDto);

                // Asignar campos de auditoría (ejemplo, ajusta según tu lógica de obtención de usuario/IP)
                // string usuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "sistema";
                // string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "N/A";
                string usuario = "test_user"; // Reemplaza con lógica real de autenticación/obtención de usuario
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"; // Reemplaza con lógica real de obtención de IP

                var actualizado = await _parametroServicio.ActualizarAsync(id, parametroModel);
                if (!actualizado)
                {
                    _logger.LogWarning($"Parámetro con ID {id} no encontrado o no se pudo actualizar.");
                    return NotFound($"Parámetro con ID {id} no encontrado o ya estaba inactivo.");
                }

                return NoContent(); // 204 No Content indica éxito sin devolver un cuerpo
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el parámetro con ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor al actualizar el parámetro.");
            }
        }

        /// <summary>
        /// Elimina lógicamente un parámetro (cambia su estado de auditoría a inactivo).
        /// </summary>
        /// <param name="id">El ID del parámetro a eliminar.</param>
        /// <returns>NoContent si la eliminación fue exitosa, o NotFound.</returns>
        [HttpDelete("{id}")] // DELETE /api/Parametro/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EliminarParametro([FromRoute] int id)
        {
            try
            {
                // Asignar campos de auditoría (ejemplo, ajusta según tu lógica de obtención de usuario/IP)
                // string usuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "sistema";
                // string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "N/A";
                string usuario = "test_user"; // Reemplaza con lógica real de autenticación/obtención de usuario
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"; // Reemplaza con lógica real de obtención de IP

                var eliminado = await _parametroServicio.EliminarAsync(id);
                if (!eliminado)
                {
                    _logger.LogWarning($"Parámetro con ID {id} no encontrado o ya estaba eliminado/inactivo.");
                    return NotFound($"Parámetro con ID {id} no encontrado o ya estaba inactivo.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar lógicamente el parámetro con ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor al eliminar el parámetro.");
            }
        }

        /// <summary>
        /// Elimina físicamente un parámetro de la base de datos. ¡Usar con precaución!
        /// </summary>
        /// <param name="id">El ID del parámetro a eliminar físicamente.</param>
        /// <returns>NoContent si la eliminación fue exitosa, o NotFound.</returns>
        [HttpDelete("fisico/{id}")] // DELETE /api/Parametro/fisico/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EliminarParametroFisico([FromRoute] int id)
        {
            try
            {
                var parametroEliminado = await _parametroServicio.EliminarFisicoAsync(id);
                if (parametroEliminado == null)
                {
                    _logger.LogWarning($"Parámetro con ID {id} no encontrado para eliminación física.");
                    return NotFound($"Parámetro con ID {id} no encontrado.");
                }

                _logger.LogInformation($"Parámetro con ID {id} eliminado físicamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar físicamente el parámetro con ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor al eliminar físicamente el parámetro.");
            }
        }
    }
}
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AnhApi.Esquemas;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RepresentanteEntidadController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RepresentanteEntidadController> _logger;
        private readonly IServicioRepresentanteEntidad _servicioRepresentanteEntidad;

        public RepresentanteEntidadController(
            IMapper mapper,
            ILogger<RepresentanteEntidadController> logger,
            IServicioRepresentanteEntidad servicioRepresentanteEntidad)
        {
            _servicioRepresentanteEntidad = servicioRepresentanteEntidad ?? throw new ArgumentNullException(nameof(servicioRepresentanteEntidad));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacionResultado<EsqRepresentanteEntidad>), StatusCodes.Status200OK)]
        [EnableRateLimiting("fijo")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)] // Demasiadas peticiones
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Petición incorrecta paginacion erronea
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // No autorizado
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Error Interno del servidor
        public async Task<ActionResult<PaginacionResultado<EsqRepresentanteEntidad>>> ObtenerTodosRepresentantes([FromQuery] PaginacionParametros paginacion)
        {
            
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var repPag = await _servicioRepresentanteEntidad.ObtenerTodosPagAsync(paginacion);
                return Ok(repPag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al listar representantes");
                return StatusCode(500, $"Error interno");
            }
        }

        [HttpGet("entidad/{idEntidad}")]
        [ProducesResponseType(typeof(IEnumerable<EsqRepresentanteEntidad>), StatusCodes.Status200OK)]
        [EnableRateLimiting("fijo")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)] // Demasiadas peticiones
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // idEntidad erroneo
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // No autorizado
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Error Interno del servidor
        public async Task<ActionResult<IEnumerable<EsqRepresentanteEntidad>>> ObtenerRepresentantesPorEntidad([FromRoute] Guid idEntidad)
        {
            try
            {
                if (idEntidad == Guid.Empty)
                {
                    ModelState.AddModelError("idEntidad", "El id de la entidad no puede ser vacío.");
                    return BadRequest(ModelState);
                }
                var representantes = await _servicioRepresentanteEntidad.Listar(idEntidad);
                var representantesDto = _mapper.Map<IEnumerable<EsqRepresentanteEntidad>>(representantes);
                return Ok(representantesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los representantes de la entidad con Guid {idEntidad}");
                return StatusCode(500, $"Error interno");
            }
        }

    }
}

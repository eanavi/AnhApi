using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using AnhApi.Esquemas;
using AnhApi.Modelos.prm;
using AutoMapper;
using AnhApi.Interfaces;


namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocalidadController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IServicioLocalidad _servicioLocalidad;

        public LocalidadController(
            IServicioLocalidad servicioLocalidad,
            IMapper mapper,
            ILogger<LocalidadController> logger
            )
        {
            _servicioLocalidad = servicioLocalidad ?? throw new ArgumentNullException(nameof(servicioLocalidad));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacionResultado<LocalidadListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EnableRateLimiting("fijo")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]//demasiadas peticiones
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//Paginacion erronea
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]//no autorizado
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginacionResultado<LocalidadListado>>> ObtenerTodosPagAsyng([FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var localidadPag = await _servicioLocalidad.ObtenerTodosPagAsync(paginacion);
                if (localidadPag == null)
                {
                    return NotFound("No se encontraron localidades");
                }
                var localidadesLista = _mapper.Map<IEnumerable<LocalidadListado>>(localidadPag.Elementos);
                var resultado = new PaginacionResultado<LocalidadListado>
                {
                    Elementos = localidadesLista,
                    TotalRegistros = localidadPag.TotalRegistros,
                    PaginaActual = localidadPag.PaginaActual,
                    TamanoPagina = localidadPag.TamanoPagina,
                };
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener localidades paginadas");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }


        [HttpGet("buscar/{criterio:alpha}")]
        [ProducesResponseType(typeof(IEnumerable<LocalidadListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EnableRateLimiting("fijo")]
        public async Task<ActionResult<IEnumerable<LocalidadListado>>> BuscarLocalidades(string criterio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criterio) || criterio.Length < 3)
                {
                    return BadRequest("El criterio de búsqueda debe tener al menos 3 caracteres.");
                }
                var localidades = await _servicioLocalidad.Buscar(criterio);
                if (localidades == null || !localidades.Any())
                {
                    return NotFound("No se encontraron localidades que coincidan con el criterio proporcionado.");
                }
                var localidadesLista = _mapper.Map<IEnumerable<LocalidadListado>>(localidades);
                return Ok(localidadesLista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar localidades con criterio {criterio}", criterio);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }


        [HttpGet("jerarquia/{criterio:alpha}")]
        [ProducesResponseType(typeof(IEnumerable<Jerarquia>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Jerarquia>>> BuscarJerarquia(string criterio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criterio) || criterio.Length < 3)
                {
                    return BadRequest("El criterio de búsqueda debe tener al menos 3 caracteres.");
                }
                var jerarquia = await _servicioLocalidad.BuscarJerarquia(criterio);
                if (jerarquia == null || !jerarquia.Any())
                {
                    return NotFound("No se encontraron jerarquías que coincidan con el criterio proporcionado.");
                }
                return Ok(jerarquia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar jerarquía de localidades con criterio {criterio}", criterio);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(EsqLocalidad), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqLocalidad>> CrearLocalidad([FromBody] LocalidadCreacion localidadCreacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var localidad = _mapper.Map<Localidad>(localidadCreacion);
                var resultado = await _servicioLocalidad.CrearAsync(localidad);
                var localidadEsquema = _mapper.Map<EsqLocalidad>(resultado);
                return CreatedAtAction(nameof(ObtenerTodosPagAsyng), new { id = localidadEsquema.IdLocalidad }, localidadEsquema);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la localidad");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarLocalidad([FromRoute] int id, [FromBody] EsqLocalidad localidadEsquema)
        {
            if (id <= 0 || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var localidad = _mapper.Map<Localidad>(localidadEsquema);
                localidad.id_localidad = id; // Aseguramos que el ID coincida
                var resultado = await _servicioLocalidad.ActualizarAsync(id, localidad);
                if (resultado == null)
                {
                    return NotFound($"No se encontró la localidad con ID {id}");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la localidad con ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }
    }
}

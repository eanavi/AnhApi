using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AnhApi.Modelos.prm;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MunicipioController : ControllerBase
    {
        private readonly IServicioMunicipio _servMunicipio;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public MunicipioController(
            IServicioMunicipio servMunicipio,
            IMapper mapper,
            ILogger<MunicipioController> logger
            )
        {
            _servMunicipio = servMunicipio ?? throw new ArgumentNullException(nameof(servMunicipio));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacionResultado<MunicipioListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EnableRateLimiting("fijo")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]//demasiadas peticiones
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//Paginacion erronea
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]//no autorizado
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginacionResultado<MunicipioListado>>> ObtenerTodosPagAsyng([FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var municipioPag = await _servMunicipio.ObtenerTodosPagAsync(paginacion);
                if (municipioPag == null)
                {
                    return NotFound("No se encontraron municipios");
                }

                var municipiosLista = _mapper.Map<IEnumerable<MunicipioListado>>(municipioPag.Elementos);
                var resultado = new PaginacionResultado<MunicipioListado>
                {
                    Elementos = municipiosLista,
                    TotalRegistros = municipioPag.TotalRegistros,
                    PaginaActual = municipioPag.PaginaActual,
                    TamanoPagina = municipioPag.TamanoPagina,
                    TotalPaginas = (int)Math.Ceiling((double)municipioPag.TotalRegistros / paginacion.TamanoPagina)
                };
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los municipios");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrio un error al procesar la solicitud");
            }
        }

        [HttpGet("buscar/{criterio:alpha}")]
        [ProducesResponseType(typeof(IEnumerable<MunicipioListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MunicipioListado>>> Buscar(string criterio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criterio))
                {
                    return BadRequest("El criterio de busqueda no puede estar vacio");
                }

                var municipios = await _servMunicipio.Buscar(criterio);

                if (municipios == null || !municipios.Any())
                {
                    return NotFound("No se encontraron municipios con el criterio especificado");
                }
                var municipiosLista = _mapper.Map<IEnumerable<MunicipioListado>>(municipios);
                return Ok(municipiosLista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar municipios con criterio {criterio}", criterio);
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrio un error al procesar la solicitud");
            }
        }

        [HttpGet("jerarquia/{criterio:alpha}")]
        [ProducesResponseType(typeof(IEnumerable<EstructuraMunicipio>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EstructuraMunicipio>>> jerarquia(string criterio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criterio))
                {
                    return BadRequest("El criterio de búsqueda no puede estar vacío.");
                }

                var estructura = await _servMunicipio.BuscarJerarquia(criterio);
                if (estructura == null || !estructura.Any())
                {
                    return NotFound($"No se encontró una estructura adecuada para el criterio {criterio}");
                }
                return Ok(estructura);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar estructura con el criterio: {Criterio}", criterio);
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error al procesar la solicitud.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EsqMunicipio), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqMunicipio>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var municipio = await _servMunicipio.ObtenerPorIdAsync(id);
                if (municipio == null)
                {
                    return NotFound($"No se encontro el municipio con el ID {id}");
                }
                var municipioEsq = _mapper.Map<EsqMunicipio>(municipio);
                return Ok(municipioEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el municipio con ID {id}.");
                return StatusCode(500, "Error interno del servidor al obtener el municipio.");
            }

        }

        [HttpPost]
        [ProducesResponseType(typeof(EsqMunicipio), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqMunicipio>> CrearAsync([FromBody] MunicipioCreacion municipioCreacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var municipio = _mapper.Map<Municipio>(municipioCreacion);
                var nuevoMunicipio = await _servMunicipio.CrearAsync(municipio);
                var municipioEsq = _mapper.Map<EsqMunicipio>(nuevoMunicipio);
                return CreatedAtAction(nameof(ObtenerPorIdAsync), new { id = municipioEsq.IdMunicipio }, municipioEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el municipio.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor al crear el municipio.");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarAsync([FromRoute] int id, [FromBody] EsqMunicipio municipioEsq)
        {
            try
            {
                if (id != municipioEsq.IdMunicipio)
                {
                    return BadRequest("El ID del municipio no coincide con el proporcionado en la URL.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var municipio = _mapper.Map<Municipio>(municipioEsq);
                var actualizado = await _servMunicipio.ActualizarAsync(id, municipio);
                if (actualizado == null)
                {
                    return NotFound($"No se encontró el municipio con ID {id} para actualizar.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el municipio con ID {Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor al actualizar el municipio.");
            }
        }

    }
}

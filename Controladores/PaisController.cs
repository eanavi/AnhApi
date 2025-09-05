using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AnhApi.Modelos.prm;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaisController : ControllerBase
    {
        private readonly IServicioPais _servPais;
        private readonly IMapper _mapper;
        private readonly ILogger<PaisController> _logger;

        public PaisController(IServicioPais servPais, IMapper mapper, ILogger<PaisController> logger)
        {
            _servPais = servPais ?? throw new ArgumentNullException(nameof(servPais));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaisListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginacionResultado<PaisListado>>> ObtenerTodosPagAsync([FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var paisesPag = await _servPais.ObtenerTodosPagAsync(paginacion);
                if (paisesPag == null)
                {
                    return NotFound("No se encontraron países.");
                }

                var PaisesLista = _mapper.Map<IEnumerable<PaisListado>>(paisesPag.Elementos);
                var resultado = new PaginacionResultado<PaisListado>
                {
                    Elementos = PaisesLista,
                    TotalRegistros = paisesPag.TotalRegistros,
                    PaginaActual = paisesPag.PaginaActual,
                    TamanoPagina = paisesPag.TamanoPagina,
                    TotalPaginas = (int)Math.Ceiling((double)paisesPag.TotalRegistros / paginacion.TamanoPagina)
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los países paginados.");
                return StatusCode(500, "Error interno del servidor al obtener los países.");
            }
        }


        /// <summary>
        /// Buscar paises paginados por un criterio de busqueda.
        /// </summary>
        /// <param name="criterio">Puede ser el nombre de un pais, codigo 2 letras, codigo 3 letras o cod numerico</param>
        /// <param name="paginacion">PaginaNumero=...&TamanoPagina..</param>
        /// <returns></returns>
        [HttpGet("buscar/{criterio:alpha}")]
        [ProducesResponseType(typeof(IEnumerable<PaisListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PaisListado>>> Buscar([FromRoute] string? criterio)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var resultadoPag = await _servPais.Buscar(criterio);

                return Ok(resultadoPag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al buscar países paginados con criterio '{criterio}'.");
                return StatusCode(500, "Error interno del servidor al buscar los países.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EsqPais), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqPais>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var pais = await _servPais.ObtenerPorIdAsync(id);
                if (pais == null)
                {
                    return NotFound($"No se encontró un país con el ID {id}.");
                }
                var paisEsq = _mapper.Map<EsqPais>(pais);
                return Ok(paisEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el país con ID {id}.");
                return StatusCode(500, "Error interno del servidor al obtener el país.");
            }
        }

        [HttpGet("departamentos/{id:int}")]
        [ProducesResponseType(typeof(PaisConDepartamentosEsq), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaisConDepartamentosEsq>> ObtenerPaisConDepartamentos([FromRoute] int id)
        {
            try
            {
                var paisConD = await _servPais.ObtenerPaisConDepartamentosAsync(id);
                if (paisConD == null)
                {
                    return NotFound($"No se encontro un pais con el Id {id}");
                }
                var paisConDEsq = _mapper.Map<PaisConDepartamentosEsq>(paisConD);
                return Ok(paisConDEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el pais con id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Interno");
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(EsqPais), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqPais>> CrearPais([FromBody] PaisCreacion paisCreacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var pais = _mapper.Map<Pais>(paisCreacion);
                pais.aud_estado = 0;

                var nPais = await _servPais.CrearAsync(pais);

                var paisEsq = _mapper.Map<EsqPais>(nPais);
                return CreatedAtAction(nameof(ObtenerPorIdAsync), new { id = paisEsq.IdPais }, paisEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el país.");
                return StatusCode(500, "Error interno del servidor al crear el país.");
            }
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarPais([FromRoute] int id, [FromBody] EsqPais pais)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var paisMod = _mapper.Map<Pais>(pais);
                var resultado = await _servPais.ActualizarAsync(id, paisMod);
                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo actualizar el parametro con id {id}");
                    return NotFound($"Parametro con ID {id} no encontrado");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el parametro con ID {id}");
                return StatusCode(500, $"Error interno del servidor al actualizar el parametro con ID {id}");
            }
        }
    }
}

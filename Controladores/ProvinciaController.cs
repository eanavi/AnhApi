using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using AnhApi.Interfaces;
using AnhApi.Esquemas;
using AutoMapper;


namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProvinciaController : ControllerBase
    {
        private readonly IServicioProvincia _servProvincia;
        private readonly IMapper _mapper;
        private readonly ILogger<ProvinciaController> _logger;
        public ProvinciaController(
            IServicioProvincia servProvincia,
            IMapper mapper,
            ILogger<ProvinciaController> logger)
        {
            _servProvincia = servProvincia ?? throw new ArgumentNullException(nameof(servProvincia));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpGet]
        [ProducesResponseType(typeof(PaginacionResultado<ProvinciaListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EnableRateLimiting("fijo")] // Limita la tasa de solicitudes a este endpoint
        [ProducesResponseType(429)] // Demasiadas peticiones
        [ProducesResponseType(400)] // Paginacion erronea
        [ProducesResponseType(401)] // No autorizado
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginacionResultado<ProvinciaListado>>> ObtenerTodosPagAsync([FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var provinciasPag = await _servProvincia.ObtenerTodosPagAsync(paginacion);
                if (provinciasPag == null)
                {
                    return NotFound("No se encontraron provincias.");
                }
                var provinciasLista = _mapper.Map<IEnumerable<ProvinciaListado>>(provinciasPag.Elementos);
                var resultado = new PaginacionResultado<ProvinciaListado>
                {
                    Elementos = provinciasLista,
                    TotalRegistros = provinciasPag.TotalRegistros,
                    PaginaActual = provinciasPag.PaginaActual,
                    TamanoPagina = provinciasPag.TamanoPagina,
                    TotalPaginas = (int)Math.Ceiling((double)provinciasPag.TotalRegistros / paginacion.TamanoPagina)
                };
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las provincias paginadas.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error al procesar la solicitud.");
            }
        }


        [HttpGet("buscar/{criterio:alpha}")]
        [ProducesResponseType(typeof(IEnumerable<ProvinciaListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProvinciaListado>>> Buscar(string criterio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criterio))
                {
                    return BadRequest("El criterio de búsqueda no puede estar vacío.");
                }
                var provincias = await _servProvincia.Buscar(criterio);
                if (provincias == null || !provincias.Any())
                {
                    return NotFound("No se encontraron provincias con el criterio especificado.");
                }
                var provinciasLista = _mapper.Map<IEnumerable<ProvinciaListado>>(provincias);
                return Ok(provinciasLista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar provincias con criterio: {Criterio}", criterio);
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error al procesar la solicitud.");
            }
        }

        [HttpGet("municipios/{id:int}")]
        [ProducesResponseType(typeof(ProvConMunicipiosEsq), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProvConMunicipiosEsq>> ObtenerProvConMunicipios([FromRoute] int id)
        {
            try
            {
                var provConMuni = await _servProvincia.ObtenerProvinciaConMunicipiosAsync(id);
                if(provConMuni == null)
                {
                    return NotFound($"No se encontro una provincia con el id {id}");
                }

                var provConMesq = _mapper.Map<ProvConMunicipiosEsq>(provConMuni);
                return Ok(provConMesq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la provincia con id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "error Interno");
            }
        }


        [HttpGet("localidades/{id:int}")]
        [ProducesResponseType(typeof(ProvConLocalidadesEsq), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProvConLocalidadesEsq>> ObtenerProvConLocalidades([FromRoute] int id)
        {
            try
            {
                var provConLoc = await _servProvincia.ObtenerProvinciaConLocalidadesAsync(id);
                if(provConLoc == null)
                {
                    return NotFound($"No se encontro una provincia con el id {id}");    
                }

                var provConLocEsq = _mapper.Map<ProvConLocalidadesEsq>(provConLoc);
                return Ok(provConLocEsq);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la provinicia con id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Interno");
            }
        }

    }
}

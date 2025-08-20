using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AnhApi.Controladores
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentoEntidadController : ControllerBase
    {
        private readonly IServicioDocumentoEntidad _servicioDocumentoEntidad;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DocumentoEntidadController(
            IServicioDocumentoEntidad servicioDocumentoEntidad,
            IMapper mapper,
            ILogger<DocumentoEntidadController> logger
            )
        {
            _servicioDocumentoEntidad = servicioDocumentoEntidad ?? throw new ArgumentNullException(nameof(servicioDocumentoEntidad));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacionResultado<EsqDocumentoEntidad>), StatusCodes.Status200OK)]
        [EnableRateLimiting("fijo")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]//demasiadas peticiones
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//Paginacion erronea
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]//no autorizado
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginacionResultado<EsqDocumentoEntidad>>> ObtenerTodosPagAsyng([FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var documentoEntidadPag = await _servicioDocumentoEntidad.ObtenerTodosPagAsync(paginacion);
                if (documentoEntidadPag == null)
                {
                    return NotFound("No se encontraron documentos de entidad");
                }
                var documentosLista = _mapper.Map<IEnumerable<EsqDocumentoEntidad>>(documentoEntidadPag.Elementos);
                var resultado = new PaginacionResultado<EsqDocumentoEntidad>
                {
                    Elementos = documentosLista,
                    TotalRegistros = documentoEntidadPag.TotalRegistros,
                    PaginaActual = documentoEntidadPag.PaginaActual,
                    TamanoPagina = documentoEntidadPag.TamanoPagina
                };
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener documentos de entidad paginados");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }

        [HttpGet("documentos/{id:guid}")]
        [ProducesResponseType(typeof(DocEntidadDespliegue), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DocEntidadDespliegue>> ObtenerDocumentos([FromRoute] Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("El ID de la entidad no puede estar vacío.");
                }
                var documentos = await _servicioDocumentoEntidad.ObtenerPorEntidadIdAsync(id);
                if (documentos == null || !documentos.Any())
                {
                    return NotFound($"No se encontraron documentos para la entidad con ID {id}.");
                }
                return Ok(documentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener documentos para la entidad con ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }

    }
}

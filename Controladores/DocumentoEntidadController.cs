using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using AnhApi.Servicios;
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AnhApi.Controladores
{
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    }
}

using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AnhApi.Modelos;

namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilController : ControllerBase
    {
        private readonly IServicioPerfil _servicioPerfil;
        private readonly ILogger<PerfilController> _logger;
        private readonly IMapper _mapper;

        public PerfilController(IServicioPerfil servicioPerfil, IMapper mapper, ILogger<PerfilController> logger)
        {
            _servicioPerfil = servicioPerfil ?? throw new ArgumentNullException(nameof(servicioPerfil));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PerfilListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginacionResultado<PerfilListado>>> ObtenerTodosPagAsync([FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var perfilesPag = await _servicioPerfil.ObtenerTodosPagAsync(paginacion);
                if (perfilesPag == null)
                {
                    return NotFound("No se encontraron perfiles.");
                }
                var perfilesLista = _mapper.Map<IEnumerable<PerfilListado>>(perfilesPag.Elementos);
                var resultado = new PaginacionResultado<PerfilListado>
                {
                    Elementos = perfilesLista,
                    TotalRegistros = perfilesPag.TotalRegistros,
                    PaginaActual = perfilesPag.PaginaActual,
                    TamanoPagina = perfilesPag.TamanoPagina,
                    TotalPaginas = (int)Math.Ceiling((double)perfilesPag.TotalRegistros / paginacion.TamanoPagina)
                };
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los perfiles paginados.");
                return StatusCode(500, "Error interno del servidor al obtener los perfiles.");
            }

        }
    }
}

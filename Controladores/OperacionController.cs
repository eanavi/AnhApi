using AnhApi.Esquemas;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OperacionController :ControllerBase
    {
        private readonly ILogger<OperacionController> _logger;
        private readonly Interfaces.IServicioOperacion _servicioOperacion;
        private readonly IMapper _mapper;
        public OperacionController(ILogger<OperacionController> logger, Interfaces.IServicioOperacion servicioOperacion, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _servicioOperacion = servicioOperacion;
            _mapper = mapper;
        }
        // Aquí puedes agregar métodos para manejar las solicitudes relacionadas con el organigrama.

        [HttpGet]
        [ProducesResponseType(typeof(List<OperacionListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginacionResultado<OperacionListado>>> ObtenerTodasOperaciones([FromQuery] PaginacionParametros paginacion )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var operaciones = await _servicioOperacion.ObtenerTodosPagAsync(paginacion);
                return Ok(operaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de operaciones.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error al procesar la solicitud.");
            }
        }

    }
}

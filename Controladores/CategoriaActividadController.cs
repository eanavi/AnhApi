using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriaActividadController : ControllerBase
    {
        private readonly ILogger<CategoriaActividadController> _logger;
        private readonly Interfaces.IServicioCategoriaActividad _servicioCategoriaActividad;
        private readonly IMapper _mapper;
        public CategoriaActividadController(
            ILogger<CategoriaActividadController> logger, 
            Interfaces.IServicioCategoriaActividad servCategoriaActividad,
            IMapper mapper

            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _servicioCategoriaActividad = servCategoriaActividad ?? throw new ArgumentNullException(nameof(servCategoriaActividad));
        }
        /// <summary>
        /// Obtiene el listado de categorías de actividad.
        /// </summary>
        /// <returns>Una lista de categorías de actividad.</returns>
        [HttpGet("listado")]
        [ProducesResponseType(typeof(IEnumerable<Esquemas.CategoriaActividadListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(401)]//No autorizado
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Esquemas.CategoriaActividadListado>>> ListadoCategorias()
        {
            try
            {
                var categorias = await _servicioCategoriaActividad.ListadoCategoria();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de Categorias ");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error al procesar la solicitud.");
            }
        }
    }
}

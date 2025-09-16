using System.Security.Cryptography.X509Certificates;
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActividadController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ActividadController> _logger;
        private readonly IservicioActividad _servicioActividad;

        public ActividadController(
            IservicioActividad servicioActividad,
            IMapper mapper,
            ILogger<ActividadController> logger
        )
        {
            _servicioActividad = servicioActividad ?? throw new ArgumentNullException(nameof(servicioActividad));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacionResultado<ActividadListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)] //Demasiadas peticiones
        [ProducesResponseType(StatusCodes.Status400BadRequest)] //Peticion incorrecta
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] //No autorizado
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] //Error Interno
        public async Task<ActionResult<PaginacionResultado<ActividadListado>>> ObtenerTodasActividades([FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var actPag = await _servicioActividad.ObtenerTodosPagAsync(paginacion);

                return Ok(actPag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al listar actividades");
                return StatusCode(500, $"Error Interno");
            }
        }


        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EsqActividad), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqActividad>> ObtenerActividadPorId([FromRoute] int id)
        {
            try
            {
                var actividad = await _servicioActividad.ObtenerPorIdAsync(id);
                if(actividad == null)
                {
                    _logger.LogInformation($"Entidad con id {id} no encontrada o no activa");
                    return NotFound($"Entidad con id {id} no encontrada");
                }

                var actividadDto = _mapper.Map<EsqActividad>(actividad);
                return Ok(actividadDto);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la actividad con id {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Actividad), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//Peticion incorrecta
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]//Error interno del servidor
        public async Task<ActionResult<Actividad>> CrearActividad([FromBody] ActividadCreacion actividadEsq)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var actividad = _mapper.Map<Actividad>(actividadEsq);
                string usuario = User.Identity?.Name ?? "Sistema_api";
                string ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";

                var nuevaActividad = await _servicioActividad.CrearAsync(actividad, usuario, ipAuditoria);
                return CreatedAtAction(nameof(ObtenerActividadPorId), new { id = nuevaActividad.id_actividad }, nuevaActividad);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al crear la entidad");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarActividad(
            [FromRoute] int id,
            [FromBody] EsqActividad actividadDto
            )
        {
            try
            {
                if(id != actividadDto.IdActividad)
                {
                    return BadRequest("El Id de la ruta no coincide conel id del cuerpo de la solicitud");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var actividadM = _mapper.Map<Actividad>(actividadDto);

                string usuarioAud = User.Identity?.Name ?? "Sistema_api";
                string idAud = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";

                var resultado = await _servicioActividad.ActualizarAsync(id, actividadM, usuarioAud, idAud);

                if (!resultado)
                {
                    _logger.LogInformation($"NO se pudo actualizar la Actividad con Id {id}. podria no existir o no estar activa");
                    return NotFound($"Entidad con id {id} no encontrada o no se pudo actualizar");
                }

                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la actividad con id {id}");
                return StatusCode(500, $"Error interno del servidor al actualizar la Actividad con Id {id}");
            }
        }
    }

}
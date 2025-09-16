using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnhApi.Datos;
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EntidadController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<EntidadController> _logger;
        private readonly IServicioEntidad _servicioEntidad;

        public EntidadController(
            IServicioEntidad servicioEntidad,
            IMapper mapper,
            ILogger<EntidadController> logger)
        {
            _servicioEntidad = servicioEntidad ?? throw new ArgumentNullException(nameof(servicioEntidad));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpGet]
        [ProducesResponseType(typeof(PaginacionResultado<EntidadListado>), StatusCodes.Status200OK)]
        [EnableRateLimiting("fijo")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]//Demasiadas peticiones
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//Petición incorrecta paginacion erronea
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]//No autorizado
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]//Error Interno del servidor
        public async Task<ActionResult<PaginacionResultado<EntidadListado>>> ObtenerTodasEntidades([FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entPag = await _servicioEntidad.listarPaginado(paginacion);

                return Ok(entPag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al listar personas");
                return StatusCode(500, $"Error interno");
            }
        }


        [HttpGet("buscar/{criterio}")]
        [ProducesResponseType(typeof(PaginacionResultado<EntidadListado>), StatusCodes.Status200OK)]
        [EnableRateLimiting("fijo")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]//Demasiadas peticiones
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//Petición incorrecta paginacion erronea
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]//No autorizado
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]//Error Interno del servidor
        public async Task<ActionResult<PaginacionResultado<EntidadListado>>> BuscarEntidades([FromRoute] string? criterio, [FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var entidadesPag = await _servicioEntidad.BuscarPaginado(criterio, paginacion);
                return Ok(entidadesPag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener entidades paginadas.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor.");
            }
        }


        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EsqEntidad), StatusCodes.Status200OK)] // El DTO completo
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqEntidad>> ObtenerEntidadPorId([FromRoute] Guid id)
        {
            try
            {
                var entidad = await _servicioEntidad.ObtenerPorIdAsync(id);
                if (entidad == null)
                {
                    _logger.LogInformation($"Entidad con id {id} no encontrada o no activa");
                    return NotFound($"Entidad con id {id} no encontrada");
                }

                var entidadDto = _mapper.Map<EsqEntidad>(entidad);
                return Ok(entidadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la entidad con id {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(Entidad), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Petición incorrecta
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Error Interno del servidor
        public async Task<ActionResult<Entidad>> CrearEntidad([FromBody] EntidadCreacion entidadEsquema)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var entidad = _mapper.Map<Entidad>(entidadEsquema);

                string usuario = User.Identity?.Name ?? "sistema_api";
                string ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";

                var nuevaEntidad = await _servicioEntidad.CrearAsync(entidad, usuario, ipAuditoria);
                return CreatedAtAction(nameof(ObtenerEntidadPorId), new { id = nuevaEntidad.id_entidad }, nuevaEntidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la entidad");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpPut("{id:guid}")] // Ruta: PUT api/Entidad/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarEntidad(
            [FromRoute] Guid id,
            [FromBody] EsqEntidad entidadDto
            )
        {
            try
            {
                if (id != entidadDto.IdEntidad)
                {
                    return BadRequest("El Id de la ruta no coincide con el id del cuerpo de la solicitud");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entidadM = _mapper.Map<Entidad>(entidadDto);

                string usuarioAud = User.Identity?.Name ?? "sistema_api_mod";
                string idAud = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";

                var resultado = await _servicioEntidad.ActualizarAsync(id, entidadM, usuarioAud, idAud);

                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo actualizar la Entidad con Id {id}. podria no existir o no estar activa");
                    return NotFound($"Entidad con id {id} no encontrada o no se pudo actualizar");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la entidad con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al actualizar la Entidad con ID {id}");
            }
        }


        [HttpGet("representantes/{id:guid}")]
        [ProducesResponseType(typeof(EntidadListadoRepresentante), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EntidadListadoRepresentante>> ObtenerEntidadConRepAsync([FromRoute] Guid id)
        {
            try
            {
                var entConRep = await _servicioEntidad.EntidadConRepresentante(id);
                if (entConRep == null)
                {
                    return NotFound($"No se encontro la entidad con el id{id}");
                }

                return Ok(entConRep);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la entidad con el id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "error Interno");
            }
        }


        [HttpGet("documentos/{id:guid}")]
        [ProducesResponseType(typeof(EntidadListadoDocumentos), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EntidadListadoDocumentos>> ObtenerEntidadConDocumentosAsync([FromRoute] Guid id)
        {
            try
            {
                var entConDoc = await _servicioEntidad.EntidadDocumentos(id);
                if (entConDoc == null)
                {
                    return NotFound($"No se encontro la entidad con el id{id}");
                }
                return Ok(entConDoc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la entidad con el id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "error Interno");
            }
        }
    }
}

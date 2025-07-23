// Archivo: AnhApi.Controladores/ParametroController.cs
using AnhApi.Esquemas;
using AnhApi.Modelos;
using AnhApi.Servicios; // Asegúrate de incluir este using
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParametroController : ControllerBase
    {
        // Cambia la inyección para usar el servicio específico
        private readonly ParametroServicio _parametroServicio; // <--- CAMBIO AQUÍ
        private readonly IMapper _mapper;
        private readonly ILogger<ParametroController> _logger;

        public ParametroController(
            ParametroServicio parametroServicio, // <--- CAMBIO AQUÍ
            IMapper mapper,
            ILogger<ParametroController> logger)
        {
            _parametroServicio = parametroServicio ?? throw new ArgumentNullException(nameof(parametroServicio));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Los métodos dentro del controlador no necesitan cambiar su lógica de mapeo
        // ni el manejo de errores, ya que la interfaz del servicio es similar.

        /// <summary>
        /// Obtiene todos los parámetros activos.
        /// </summary>
        /// <returns>Una lista de ParametroEsq.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ParametroEsq>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ParametroEsq>>> ObtenerTodosParametros()
        {
            try
            {
                var parametros = await _parametroServicio.ObtenerTodosAsync();
                if (parametros == null)
                {
                    return Ok(new List<ParametroEsq>());
                }
                var parametrosEsq = _mapper.Map<IEnumerable<ParametroListado>>(parametros);
                return Ok(parametrosEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los parámetros.");
                return StatusCode(500, "Error interno del servidor al obtener parámetros.");
            }
        }

        /// <summary>
        /// Obtiene un parámetro por su ID.
        /// </summary>
        /// <param name="id">El ID del parámetro.</param>
        /// <returns>Un ParametroEsq.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ParametroEsq), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ParametroEsq>> ObtenerParametroPorId(int id)
        {
            try
            {
                var parametro = await _parametroServicio.ObtenerPorIdAsync(id);
                if (parametro == null)
                {
                    _logger.LogInformation($"Parámetro con ID {id} no encontrado o no activo.");
                    return NotFound($"Parámetro con ID {id} no encontrado.");
                }
                var parametroEsq = _mapper.Map<ParametroEsq>(parametro);
                return Ok(parametroEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el parámetro con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al obtener el parámetro con ID {id}.");
            }
        }

        [HttpGet("grupo/{grupo}")]
        [ProducesResponseType(typeof(IEnumerable<ParametroCmb>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ParametroCmb>>> ObtenerGrupo(string grupo)
        {
            try
            {
                var parametro = await _parametroServicio.ObtenerGrupo(grupo);
                if (parametro == null)
                {
                    _logger.LogInformation($"Parametro con grupo {grupo} no encontrado");
                    return NotFound($"Grupo de parametros no encontrados");
                }
                var prmCmb = _mapper.Map<IEnumerable<ParametroCmb>>(parametro);
                return Ok(prmCmb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el parametro");
                return StatusCode(500, $"Error interno del servidor al obtener el grupo {grupo}");
            }
        }

        /// <summary>
        /// Crea un nuevo parámetro.
        /// </summary>
        /// <param name="parametroCreacionDto">Los datos del parámetro a crear.</param>
        /// <returns>El ParametroEsq creado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ParametroEsq), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ParametroEsq>> CrearParametro(
            [FromBody] ParametroCreacion parametroCreacionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var parametroModel = _mapper.Map<Parametro>(parametroCreacionDto);

                parametroModel.aud_estado = 0; //El valor por defecto para activo


                var parametroCreado = await _parametroServicio.CrearAsync(parametroModel);

                var parametroEsq = _mapper.Map<ParametroEsq>(parametroCreado);

                return CreatedAtAction(nameof(ObtenerParametroPorId),
                                       new { id = parametroEsq.IdParametro },
                                       parametroEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo parámetro.");
                return StatusCode(500, "Error interno del servidor al crear el parámetro.");
            }
        }

        /// <summary>
        /// Actualiza un parámetro existente.
        /// </summary>
        /// <param name="id">El ID del parámetro a actualizar.</param>
        /// <param name="parametroEsqDto">Los datos actualizados del parámetro.</param>
        /// <returns>No Content si la actualización es exitosa.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> ActualizarParametro(
            int id,
            [FromBody] ParametroEsq parametroEsqDto)
        {
            try
            {
                if (id != parametroEsqDto.IdParametro)
                {
                    return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var parametroModel = _mapper.Map<Parametro>(parametroEsqDto);

                // No necesitamos pasar usuario/IP a este servicio
                var resultado = await _parametroServicio.ActualizarAsync(id, parametroModel);
                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo actualizar el parámetro con ID {id}. Podría no existir o no estar activo.");
                    return NotFound($"Parámetro con ID {id} no encontrado o no se pudo actualizar.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el parámetro con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al actualizar el parámetro con ID {id}.");
            }
        }

        /// <summary>
        /// Elimina lógicamente un parámetro (cambia su estado).
        /// </summary>
        /// <param name="id">El ID del parámetro a eliminar.</param>
        /// <returns>No Content si la eliminación es exitosa.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> EliminarParametro(int id)
        {
            try
            {
                // No necesitamos pasar usuario/IP a este servicio
                var resultado = await _parametroServicio.EliminarAsync(id);

                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo eliminar el parámetro con ID {id}. Podría no existir.");
                    return NotFound($"Parámetro con ID {id} no encontrado.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el parámetro con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al eliminar el parámetro con ID {id}.");
            }
        }
    }
}
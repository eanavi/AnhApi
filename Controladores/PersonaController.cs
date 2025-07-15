using AnhApi.Esquemas; 
using AnhApi.Modelos;
using AnhApi.Servicios;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnhApi.Controladores
{
    [ApiController] // Indica que es un controlador de API
    [Route("api/[controller]")] // Define la ruta base para este controlador
    public class PersonaController : ControllerBase
    {
        private readonly IGenericoServicio<Persona, Guid> _personaServicio;
        private readonly IMapper _mapper;
        private readonly ILogger<PersonaController> _logger;

        // Constructor con inyección de dependencias
        public PersonaController(
            IGenericoServicio<Persona, Guid> personaServicio,
            IMapper mapper,
            ILogger<PersonaController> logger)
        {
            _personaServicio = personaServicio ?? throw new ArgumentNullException(nameof(personaServicio));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

 

        /// <summary>
        /// Obtiene una lista de todas las personas activas (vista de listado reducido).
        /// </summary>
        /// <returns>Una lista de PersonaListado.</returns>
        [HttpGet] // Ruta: GET api/personas
        [ProducesResponseType(typeof(IEnumerable<PersonaListado>), 200)] // Indica el tipo de retorno esperado
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PersonaListado>>> ObtenerTodasPersonas()
        {
            try
            {
                var personas = await _personaServicio.ObtenerTodosAsync();
                if (personas == null)
                {
                    // Aunque el servicio debería devolver una colección vacía si no hay nada,
                    // NotFound es una opción si no se espera que exista la "colección" en absoluto.
                    return Ok(new List<PersonaListado>()); // Devolver una lista vacía en lugar de NotFound para "todos"
                }
                // Mapear la colección de modelos a la colección de DTOs de listado
                var personasListado = _mapper.Map<IEnumerable<PersonaListado>>(personas);
                return Ok(personasListado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las personas.");
                return StatusCode(500, "Error interno del servidor al obtener las personas.");
            }
        }



        /// <summary>
        /// Obtiene una persona por su ID (vista completa).
        /// </summary>
        /// <param name="id">El ID de la persona (Guid).</param>
        /// <returns>Un objeto Persona (DTO completo).</returns>
        [HttpGet("{id:guid}")] // Ruta: GET api/personas/{id} - Usamos :guid para validar el formato Guid en la ruta
        [ProducesResponseType(typeof(Persona), 200)] // El DTO completo
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Persona>> ObtenerPersonaPorId(Guid id)
        {
            try
            {
                var persona = await _personaServicio.ObtenerPorIdAsync(id);
                if (persona == null)
                {
                    _logger.LogInformation($"Persona con ID {id} no encontrada o no activa.");
                    return NotFound($"Persona con ID {id} no encontrada.");
                }
                // Mapear el modelo a un DTO completo
                var personaDto = _mapper.Map<Persona>(persona);
                return Ok(personaDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la persona con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al obtener la persona con ID {id}.");
            }
        }

        
        /// <summary>
        /// Crea una nueva persona.
        /// </summary>
        /// <param name="personaCreacionDto">Los datos de la persona a crear.</param>
        /// <returns>La Persona (DTO completo) creada.</returns>
        [HttpPost] // Ruta: POST api/personas
        [ProducesResponseType(typeof(Persona), 201)] // Devuelve el DTO completo de la persona creada
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Persona>> CrearPersona(
            [FromBody] PersonaCreacion personaCreacionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Devuelve errores de validación del DTO
                }

                var personaModel = _mapper.Map<Persona>(personaCreacionDto);

                // --- Asignación de campos de auditoría al modelo ANTES de crear ---
                // Estos campos no vienen en PersonaCreacion, pero son requeridos por el modelo de BD.
                // Aquí necesitarías una forma de obtener el usuario y la IP reales (ej. desde claims de JWT)
                personaModel.aud_usuario = User.Identity?.Name ?? "sistema_api"; // Usuario que realiza la acción
                personaModel.aud_ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"; // IP del cliente
                personaModel.aud_fecha = DateTime.UtcNow; // Fecha y hora actual UTC
                personaModel.aud_estado = 0; // O el valor por defecto que uses para "activo"

                var personaCreada = await _personaServicio.CrearAsync(
                    personaModel,
                    personaModel.aud_usuario,
                    personaModel.aud_ip); // Pasar los campos de auditoría al servicio base

                // Mapear el modelo creado (que ya tiene su ID generado y campos de auditoría)
                // al DTO completo para la respuesta.
                var personaDto = _mapper.Map<Persona>(personaCreada);

                // 201 Created: Indica que se creó un nuevo recurso y devuelve su ubicación.
                return CreatedAtAction(nameof(ObtenerPersonaPorId),
                                       new { id = personaDto.id_persona},
                                       personaDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva persona.");
                return StatusCode(500, "Error interno del servidor al crear la persona.");
            }
        }

       

        /// <summary>
        /// Actualiza una persona existente.
        /// </summary>
        /// <param name="id">El ID de la persona a actualizar.</param>
        /// <param name="personaDto">Los datos actualizados de la persona (DTO completo).</param>
        /// <returns>No Content si la actualización es exitosa.</returns>
        [HttpPut("{id:guid}")] // Ruta: PUT api/personas/{id}
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)] // Bad Request (si el ID no coincide o DTO inválido)
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)]
        public async Task<ActionResult> ActualizarPersona([FromRoute] Guid id,
            [FromBody] PersonaEsq personaDto) // Recibe el DTO completo para la actualización
        {
            try
            {
                if (id != personaDto.IdPersona)
                {
                    return BadRequest("El ID de la ruta no coincide con el ID del cuerpo de la solicitud.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var personaModel = _mapper.Map<Persona>(personaDto);

                // Los campos de auditoría ya deberían venir en personaDto si se desea actualizar,
                // o se pueden sobreescribir aquí si solo se gestionan desde el backend.
                // Si quieres que el PUT solo sea para el usuario logueado, puedes forzar aquí
                // personaModel.aud_usuario = User.Identity?.Name ?? "sistema_api_put";
                // personaModel.aud_ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
                // personaModel.aud_fecha = DateTime.UtcNow;

                var usuarioAuditoria = User.Identity?.Name ?? "sistema_api_put"; // Usuario que realiza la acción
                var ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"; // IP del cliente


                var resultado = await _personaServicio.ActualizarAsync(
                    id, personaModel, usuarioAuditoria, ipAuditoria);

                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo actualizar la persona con ID {id}. Podría no existir o no estar activa.");
                    return NotFound($"Persona con ID {id} no encontrada o no se pudo actualizar.");
                }

                return NoContent(); // 204 No Content para PUT exitoso
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la persona con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al actualizar la persona con ID {id}.");
            }
        }


        /// <summary>
        /// Elimina lógicamente una persona (cambia su estado a inactivo).
        /// </summary>
        /// <param name="id">El ID de la persona a eliminar.</param>
        /// <returns>No Content si la eliminación es exitosa.</returns>
        [HttpDelete("{id:guid}")] // Ruta: DELETE api/personas/{id}
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)]
        public async Task<ActionResult> EliminarPersona(Guid id)
        {
            try
            {
                // Auditoría para la eliminación
                var usuarioAuditoria = User.Identity?.Name ?? "sistema_api_delete";
                var ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";

                var resultado = await _personaServicio.EliminarAsync(id, usuarioAuditoria, ipAuditoria);

                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo eliminar la persona con ID {id}. Podría no existir.");
                    return NotFound($"Persona con ID {id} no encontrada.");
                }

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la persona con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al eliminar la persona con ID {id}.");
            }
        }
    }
}
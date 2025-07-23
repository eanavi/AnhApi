using AnhApi.Esquemas; 
using AnhApi.Modelos;
using AnhApi.Servicios;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnhApi.Controladores
{
    /// <summary>
    /// Rutas para Persona, estan listadas las opciones por defecto para un CRUD
    /// </summary>
    [ApiController] // Indica que es un controlador de API
    [Route("api/[controller]")] // Define la ruta base para este controlador
    [Authorize]
    public class PersonaController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PersonaController> _logger;
        private readonly PersonaServicio _personaServ;

        // Constructor con inyección de dependencias
        public PersonaController(
            PersonaServicio personaServ,
            IMapper mapper,
            ILogger<PersonaController> logger)
        {
            _personaServ = personaServ ?? throw new ArgumentNullException(nameof(personaServ));
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
                var personas = await _personaServ.ObtenerTodosAsync();
                if (personas == null)
                {
                    // Aunque el servicio debería devolver una colección vacía si no hay nada,
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
        /// Buscar personas realiza una busqueda por tres criterios diferentes: nombre, ci o fechaNacimiento 
        /// </summary>
        /// <param name="criterio">
        /// combinaciones: paterno, 1erNombre patero, 1erNombre paterno materno 
        ///                                                      1erNombre 2doNombre paterno materno
        /// carnet de identidad que son numeros
        /// fecha de nacimiento en formato dd-mm-yyyy para no confundir al enrutador</param>
        /// <returns>lista de Personas</returns>
        [HttpGet("buscar/{criterio}")]
        [ProducesResponseType(typeof(IEnumerable<PersonaListado>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PersonaListado>>> BuscarPersonas(string criterio)
        {
            try
            {
                var personas = await _personaServ.Buscar(criterio);
                if (personas == null)
                {
                    return Ok(new List<PersonaListado>());
                }
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
                var persona = await _personaServ.ObtenerPorIdAsync(id);
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

                
                var nuevaPersona = _mapper.Map<Persona>(personaCreacionDto);

                string usuarioAuditoria = User.Identity?.Name ?? "sistema_api"; // Usuario que realiza la acción
                string ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"; // IP del cliente

                var personaCreada = await _personaServ.CrearAsync(nuevaPersona, usuarioAuditoria, ipAuditoria); 
                // Pasar los campos de auditoría al servicio base

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

                //Verificar si se almacenara el nombre de usuario en el momento de la modificacion del registro
                string usuarioAuditoria = User.Identity?.Name ?? "sistema_api_put"; // Usuario que realiza la acción
                string ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"; // IP del cliente

                var resultado = await _personaServ.ActualizarAsync(id, personaModel, usuarioAuditoria, ipAuditoria);

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
                string usuarioAuditoria = User.Identity?.Name ?? "sistema_api_delete";
                string ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";

                var resultado = await _personaServ.EliminarAsync(id, usuarioAuditoria, ipAuditoria);

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
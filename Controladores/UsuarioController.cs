using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnhApi.Esquemas; // Para los DTOs de Usuario
using AnhApi.Interfaces; // Para UsuarioServicio (que hereda de GenericoServicio)
using AnhApi.Modelos;   // Para el modelo de base de datos Usuario
using AutoMapper;       // Para IMapper
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using Microsoft.AspNetCore.Mvc; // Para el controlador y los atributos de HTTP
using Microsoft.Extensions.Logging; // Para ILogger

namespace AnhApi.Controladores
{
    /// <summary>
    /// Provee un las rutas para el registro de un usuario, las rutas de autenticacion se encuentran en el controlador de autorizacion
    /// Todas las rutas requieren autenticacion, algunas rutas deberian estar reservadas para determinados perfiles
    /// </summary>
    [ApiController] // Indica que es un controlador de API
    [Route("api/[controller]")] // Define la ruta base para este controlador
    [Authorize] // Protege todas las rutas de este controlador por defecto (requiere JWT)
    public class UsuarioController : ControllerBase
    {
        private readonly IServicioUsuario _usuarioServicio; // Inyectamos el servicio específico de Usuario
        private readonly IMapper _mapper;
        private readonly ILogger<UsuarioController> _logger;

        // Constructor con inyección de dependencias
        public UsuarioController(
            IServicioUsuario usuarioServicio,
            IMapper mapper,
            ILogger<UsuarioController> logger)
        {
            _usuarioServicio = usuarioServicio ?? throw new ArgumentNullException(nameof(usuarioServicio));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene una lista de todos los usuarios activos (vista de listado reducido).
        /// Requiere autenticación.
        /// </summary>
        /// <returns>Una lista de UsuarioListado.</returns>
        [HttpGet] // Ruta: GET api/usuarios
        [ProducesResponseType(typeof(IEnumerable<UsuarioListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request (validación de modelo)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UsuarioListado>>> ObtenerTodosUsuarios(
            [FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Devuelve errores de validación del modelo
                }

                var usuariosPag = await _usuarioServicio.ObtenerTodosPagAsync(paginacion); // Método heredado de GenericoServicio
                if (usuariosPag == null)
                {
                    return Ok(new List<UsuarioListado>());
                }
                var usuariosListado = _mapper.Map<IEnumerable<UsuarioListado>>(usuariosPag.Elementos);
                var resultado = new PaginacionResultado<UsuarioListado>
                {
                    Elementos = usuariosListado,
                    TotalRegistros = usuariosPag.TotalRegistros,
                    PaginaActual = usuariosPag.PaginaActual,
                    TamanoPagina = usuariosPag.TamanoPagina,
                    TotalPaginas = usuariosPag.TotalPaginas
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios.");
                return StatusCode(500, "Error interno del servidor al obtener usuarios.");
            }
        }

        /// <summary>
        /// Obtiene un usuario por su ID (vista completa).
        /// Requiere autenticación.
        /// </summary>
        /// <param name="id">El ID del usuario (long).</param>
        /// <returns>Un objeto UsuarioEsq (DTO completo).</returns>
        [HttpGet("{id:long}")] // Ruta: GET api/usuarios/{id}
        [ProducesResponseType(typeof(EsqUsuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqUsuario>> ObtenerUsuarioPorId([FromRoute] long id)
        {
            try
            {
                var usuario = await _usuarioServicio.ObtenerPorIdAsync(id); // Método heredado de GenericoServicio
                if (usuario == null)
                {
                    _logger.LogInformation($"Usuario con ID {id} no encontrado o no activo.");
                    return NotFound($"Usuario con ID {id} no encontrado.");
                }
                var usuarioDto = _mapper.Map<EsqUsuario>(usuario);
                return Ok(usuarioDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el usuario con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al obtener el usuario con ID {id}.");
            }
        }

        /// <summary>
        /// Obtiene un usuario por su login (vista completa).
        /// Requiere autenticación.
        /// </summary>
        /// <param name="login">El login del usuario que puede ser nombre_de_usuario o correo.</param>
        /// <returns>Un objeto UsuarioEsq (DTO completo).</returns>
        [HttpGet("login/{login:alpha}")] // Ruta: GET api/usuarios/login/{login}
        [ProducesResponseType(typeof(EsqUsuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqUsuario>> ObtenerUsuarioPorLogin([FromRoute] string login)
        {
            try
            {
                var usuario = await _usuarioServicio.ObtenerPorLogin(login); // Método específico de UsuarioServicio
                if (usuario == null)
                {
                    _logger.LogInformation($"Usuario con login '{login}' no encontrado.");
                    return NotFound($"Usuario con login '{login}' no encontrado.");
                }
                // ObtenerPorLogin ya devuelve UsuarioEsq, no necesita mapeo adicional aquí
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el usuario con login '{login}'.");
                return StatusCode(500, $"Error interno del servidor al obtener el usuario con login '{login}'.");
            }
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// Requiere autenticación y el rol 'Administrador'.
        /// </summary>
        /// <param name="usuarioCreacionDto">Los datos del usuario a crear.</param>
        /// <returns>El UsuarioEsq creado.</returns>
        [HttpPost] // Ruta: POST api/usuarios
        //[Authorize(Roles = "Administrador")] // Solo administradores pueden crear usuarios
        [ProducesResponseType(typeof(EsqUsuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request (validación de modelo)
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden (si no tiene el rol)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqUsuario>> CrearUsuario(
            [FromBody] UsuarioCreacion usuarioCreacionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Devuelve errores de validación del DTO
                }

                // Obtener datos de auditoría del contexto de la solicitud
                var usuarioAuditoria = User.Identity?.Name ?? "sistema_api"; // Usuario que realiza la acción
                var ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"; // IP del cliente

                // Llama al método específico de UsuarioServicio que maneja el hashing y auditoría
                var usuarioCreado = await _usuarioServicio.CrearUsuario(
                    usuarioCreacionDto, usuarioAuditoria, ipAuditoria);

                // 201 Created: Indica que se creó un nuevo recurso y devuelve su ubicación.
                return CreatedAtAction(nameof(ObtenerUsuarioPorId),
                                       new { id = usuarioCreado.IdUsuario },
                                       usuarioCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo usuario.");
                return StatusCode(500, "Error interno del servidor al crear el usuario.");
            }
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// Requiere autenticación y el rol 'Administrador'.
        /// </summary>
        /// <param name="id">El ID del usuario a actualizar.</param>
        /// <param name="usuarioEsqDto">Los datos actualizados del usuario (DTO completo).</param>
        /// <returns>No Content si la actualización es exitosa.</returns>
        [HttpPut("{id:long}")] // Ruta: PUT api/usuarios/{id}
        [Authorize(Roles = "Administrador")] // Solo administradores pueden actualizar usuarios
        [ProducesResponseType(StatusCodes.Status204NoContent)] // No Content
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request (si el ID no coincide o DTO inválido)
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarUsuario(
            [FromRoute] long id,
            [FromBody] EsqUsuario usuarioEsqDto)
        {
            try
            {
                if (id != usuarioEsqDto.IdUsuario)
                {
                    return BadRequest("El ID de la ruta no coincide con el ID del cuerpo de la solicitud.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuarioModel = _mapper.Map<Usuario>(usuarioEsqDto);

                // Obtener datos de auditoría para la actualización
                var usuarioAuditoria = User.Identity?.Name ?? "sistema_api_put";
                var ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";

                var resultado = await _usuarioServicio.ActualizarAsync(
                    id, usuarioModel, usuarioAuditoria, ipAuditoria); // Método heredado de GenericoServicio

                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo actualizar el usuario con ID {id}. Podría no existir o no estar activo.");
                    return NotFound($"Usuario con ID {id} no encontrado o no se pudo actualizar.");
                }

                return NoContent(); // 204 No Content para PUT exitoso
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el usuario con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al actualizar el usuario con ID {id}.");
            }
        }

        /// <summary>
        /// Elimina lógicamente un usuario (cambia su estado a inactivo).
        /// Requiere autenticación y el rol 'Administrador'.
        /// </summary>
        /// <param name="id">El ID del usuario a eliminar.</param>
        /// <returns>No Content si la eliminación es exitosa.</returns>
        [HttpDelete("{id:long}")] // Ruta: DELETE api/usuarios/{id}
        [Authorize(Roles = "Administrador")] // Solo administradores pueden eliminar
        [ProducesResponseType(StatusCodes.Status204NoContent)] // No Content
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EliminarUsuario([FromRoute] long id)
        {
            try
            {
                // Obtener datos de auditoría para la eliminación
                var usuarioAuditoria = User.Identity?.Name ?? "sistema_api_delete";
                var ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";

                var resultado = await _usuarioServicio.EliminarAsync(
                    id, usuarioAuditoria, ipAuditoria); // Método heredado de GenericoServicio

                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo eliminar el usuario con ID {id}. Podría no existir.");
                    return NotFound($"Usuario con ID {id} no encontrado.");
                }

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el usuario con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al eliminar el usuario con ID {id}.");
            }
        }

        /// <summary>
        /// Elimina físicamente un usuario de la base de datos.
        /// Requiere autenticación y el rol 'Administrador'.
        /// ¡Usar con extrema precaución!
        /// </summary>
        /// <param name="id">El ID del usuario a eliminar físicamente.</param>
        /// <returns>No Content si la eliminación es exitosa.</returns>
        [HttpDelete("fisico/{id:long}")] // Ruta: DELETE api/usuarios/fisico/{id}
        //[Authorize(Roles = "Administrador")] // Solo administradores pueden eliminar físicamente
        [ProducesResponseType(StatusCodes.Status204NoContent)] // No Content
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EliminarUsuarioFisico([FromRoute] long id)
        {
            try
            {
                var usuarioEliminado = await _usuarioServicio.EliminarFisicoAsync(id); // Método heredado de GenericoServicio

                if (usuarioEliminado == null)
                {
                    _logger.LogInformation($"No se pudo eliminar físicamente el usuario con ID {id}. No encontrado.");
                    return NotFound($"Usuario con ID {id} no encontrado.");
                }

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar físicamente el usuario con ID {id}.");
                return StatusCode(500, $"Error interno del servidor al eliminar físicamente el usuario con ID {id}.");
            }
        }
    }
}
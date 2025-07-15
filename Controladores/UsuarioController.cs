using AnhApi.Esquemas;
using AnhApi.Modelos;
using AnhApi.Servicios;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;


namespace AnhApi.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IGenericoServicio<Modelos.Usuario, Guid> _servicioUsuario;
        private readonly IMapper _mapper;
        private readonly ILogger<UsuarioController> _logger;

        #region Constructor
        public UsuarioController(
            IGenericoServicio<Usuario, Guid> servicioUsuario, 
            IMapper mapper, 
            ILogger<UsuarioController> logger)
        {
            _servicioUsuario = servicioUsuario ?? throw new ArgumentNullException(nameof(servicioUsuario));
            _mapper = mapper?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
        }
        #endregion Constructor

        #region ListarTodo
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsuarioEsq>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<UsuarioEsq>>> ListarTodo()
        {
            var usuarios = await _servicioUsuario.ObtenerTodosAsync();
            if (usuarios == null || !usuarios.Any())
            {
                _logger.LogInformation("No se encontraron usuarios");
                return NotFound();
            }
            var usuariosListado = _mapper.Map<IEnumerable<UsuarioListado>>(usuarios);
            return Ok(usuarios);
        }
        #endregion ListarTodo

        #region ObtenerPorId
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Usuario),200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UsuarioEsq>> ObtenerPorId(Guid id)
        {
            try
            {
                var usuario = await _servicioUsuario.ObtenerPorIdAsync(id);
                if (usuario == null)
                {
                    _logger.LogInformation("Usuario con ID {Id} no encontrado", id);
                    return NotFound();
                }
                var usuarioEsq = _mapper.Map<UsuarioEsq>(usuario);
                return Ok(usuarioEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID {Id}", id);

                // Log the exception (not shown here)
                return StatusCode(500, "Error interno del servidor");
            }
        }
        #endregion ObtenerPorId

        #region CrearUsuario

        [HttpPost]
        [ProducesResponseType(typeof(UsuarioEsq), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UsuarioEsq>> CrearUsuario([FromBody] UsuarioCreacion usuarioCreacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Modelo de usuario invalido: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }
                var usuario = _mapper.Map<Usuario>(usuarioCreacion);
                usuario.aud_usuario = User.Identity?.Name ?? "Sistema";//verificar el usuario y recuperar del token
                usuario.aud_ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocido"; // IP del cliente
                usuario.aud_fecha = DateTime.UtcNow; // Fecha de creación
                usuario.aud_estado = 0;

                var nuevoUsuario = await _servicioUsuario.CrearAsync(usuario, User.Identity?.Name ?? "Sistema", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocido");
                var usuarioEsq = _mapper.Map<UsuarioEsq>(nuevoUsuario);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = usuarioEsq.IdUsuario }, usuarioEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        #endregion CrearUsuario

        #region ActualizarUsuario
        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)] // Sin contenido
        [ProducesResponseType(400)] // No valido
        [ProducesResponseType(404)] // No encontrado
        [ProducesResponseType(500)] // Error interno del servidor
        public async Task<ActionResult> ActualizarUsuario([FromRoute] Guid id, [FromBody] UsuarioEsq usuarioEsq)
        {
            try
            {
                if (id != usuarioEsq.IdUsuario)
                {
                    _logger.LogWarning("ID del usuario no coincide con el ID en el cuerpo de la solicitud");
                    return BadRequest("El ID del usuario no coincide con el ID en el cuerpo de la solicitud");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Modelo de usuario invalido: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }
                var usuario = _mapper.Map<Usuario>(usuarioEsq);
                usuario.aud_usuario = User.Identity?.Name ?? "Sistema";
                usuario.aud_ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocido";
                usuario.aud_fecha = DateTime.UtcNow;
                var actualizado = await _servicioUsuario.ActualizarAsync(id, usuario, User.Identity?.Name ?? "Sistema", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocido");
                if (!actualizado)
                {
                    _logger.LogWarning("No se pudo actualizar el usuario con ID {Id}", id);
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        #endregion ActualizarUsuario

        #region EliminarUsuario
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)] // Sin contenido
        [ProducesResponseType(404)] // No encontrado
        [ProducesResponseType(500)] // Error interno del servidor
        public async Task<ActionResult> EliminarUsuario([FromRoute] Guid id)
        {
            try
            {
                var usuario = User.Identity?.Name ?? "Sistema";
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
                var eliminado = await _servicioUsuario.EliminarAsync(id, usuario, ip);
                if (!eliminado)
                {
                    _logger.LogWarning("No se pudo eliminar el usuario con ID {Id}", id);
                    return NotFound($"Persona con ID {id} no encontrada.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el usuario con ID {id}");
                return StatusCode(500, $"Error interno del servidor");
            }
        }
        #endregion EliminarUsuario
    }
}

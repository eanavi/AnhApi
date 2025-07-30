using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AnhApi.Esquemas;
using AnhApi.Servicios;
using Microsoft.AspNetCore.Authorization;

namespace AnhApi.Controladores
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServicio _authServicio;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthServicio authServicio, ILogger<AuthController> logger)
        {
            _authServicio = authServicio ?? throw new ArgumentNullException(nameof(authServicio));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpPost("login")]//Ruta completa api/auth/login
        [ProducesResponseType(typeof(LoginResponse),StatusCodes.Status200OK)] //ok
        [ProducesResponseType(StatusCodes.Status400BadRequest)] //Error (validacion de modelo)
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] //No autorizado (credenciales ivalidas)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] //Error Interno de servidor
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Solicitud de login invalida");
                    return BadRequest(ModelState);
                }

                var response = await _authServicio.AutenticarUsuario(request);

                if (response == null)
                {
                    _logger.LogWarning($"Itento de Login fallido usuario {request.Login}");
                    return Unauthorized(new { message = "Credenciales Ivalidas" });
                }

                return Ok(response);

            } 
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error inesperado usuario '{request.Login}'");
                return StatusCode(500, "Error interno");

            }
        }

    }
}

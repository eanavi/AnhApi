using AnhApi.Esquemas;
using AnhApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private readonly IServicioAuth _servicioAuth;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IServicioAuth authServicio, ILogger<AuthController> logger)
        {
            _servicioAuth = authServicio ?? throw new ArgumentNullException(nameof(authServicio));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpPost("login")]//Ruta completa api/auth/login
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)] //ok
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

                var response = await _servicioAuth.AutenticarUsuario(request);

                if (response == null)
                {
                    _logger.LogWarning($"Itento de Login fallido usuario {request.Login}");
                    return Unauthorized(new { message = "Credenciales Ivalidas" });
                }

                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado usuario '{request.Login}'");
                return StatusCode(500, "Error interno");

            }
        }

    }
}

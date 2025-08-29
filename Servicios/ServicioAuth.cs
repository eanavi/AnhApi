using AnhApi.Datos;
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.DirectoryServices.Protocols;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;


namespace AnhApi.Servicios
{
    public class ServicioAuth : IServicioAuth    
    {
        private readonly ContextoAppBD _contextoBd;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServicioAuth> _logger;
        private readonly LdapOptions _ldapOptions;
        private readonly string _dominio = "@ANH.GOB.BO";

        public ServicioAuth(ContextoAppBD context, IConfiguration configuration, ILogger<ServicioAuth> logger, IOptions<LdapOptions> ldapOptionsAccessor)
        {
            _contextoBd = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ldapOptions = ldapOptionsAccessor.Value ?? throw new ArgumentNullException(nameof(ldapOptionsAccessor), "LdapOptions no pueden ser nulas.");

            if (string.IsNullOrEmpty(_ldapOptions.DirectoryEntry) ||
               string.IsNullOrEmpty(_ldapOptions.Domain) ||
               string.IsNullOrEmpty(_ldapOptions.Filter))
            {
                throw new ArgumentException("Configuracion de LDAP incompleta.");
            }
        }


        public async Task<LoginResponse?> AutenticarUsuario(LoginRequest request)
        {
            try
            {

                bool esUsuarioLdap = !request.Login.Contains("@") || request.Login.Contains($"{_ldapOptions.Domain}", StringComparison.OrdinalIgnoreCase);
                bool esUsuarioLocal = request.Login.Contains("@");

                Modelos.Usuario? usuarioAutenticado = null; //Podria ser un usuario de la BD
                string rolAutenticacion = "UsuarioGenerico"; //Rol por defeto si no se especifica
                Guid idUsuario = Guid.Empty;
                string idUsuarioPToken = Guid.Empty.ToString(); // Id para el Token, puede ser Guid o el int de la DB

                if (esUsuarioLocal)
                {
                    var usuarioLocal = await _contextoBd.Usuarios.FirstOrDefaultAsync(u => u.login == request.Login);

                    if (usuarioLocal != null && BCrypt.Net.BCrypt.Verify(request.Clave, usuarioLocal.clave))
                    {
                        usuarioAutenticado = usuarioLocal;
                        //buscar el rol en la base de datos
                        var perfil = await _contextoBd.Perfiles.FirstOrDefaultAsync(p => p.IdPerfil == usuarioLocal.id_perfil);
                        if (perfil != null)
                        {
                            rolAutenticacion = perfil.Descripcion;
                        }
                        idUsuarioPToken = usuarioAutenticado.id_usuario.ToString();
                    }
                    else
                    {
                        _logger.LogWarning($"Login local fallido para el usuario {request.Login}");
                    }
                } 
                else if (esUsuarioLdap)
                {
                    string nombreUsuarioLdap = request.Login;
                    if (!nombreUsuarioLdap.Contains("@anh.gob.bo", StringComparison.OrdinalIgnoreCase))
                        nombreUsuarioLdap = nombreUsuarioLdap.Trim().ToUpper() + _dominio;

                    //string nombreUsuarioLdap = request.Login.Replace($"@{_ldapOptions.Domain}", "");
                    if(await AutenticarJson(nombreUsuarioLdap, request.Clave))
                    //if(await AutenticarConLdapAsync(nombreUsuarioLdap, request.Clave))
                    {
                        //Actualmente el servicio solo verifica que el nombre se encuentre en la base de datos, no verifica la clave
                        //la clave debe ser verificada con el Active Directory
                        var usuarioAD = await _contextoBd.Usuarios.FirstOrDefaultAsync(u => u.login.ToUpper() == nombreUsuarioLdap);

                        if (usuarioAD == null)
                        {//Esta en Active Directory  pero no en la bd ***preguntar
                            idUsuarioPToken = Guid.NewGuid().ToString();
                            rolAutenticacion = "UsuarioAD";
                            // aqui se podria grabar pero necesitamos a la persona asociada
                        }
                        else
                        {
                            usuarioAutenticado = usuarioAD;
                            idUsuarioPToken = usuarioAD.id_usuario.ToString();
                            rolAutenticacion = "UsuarioADExistente";//cargar el rol
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Login LDAP Fallido para el usuario: {request.Login} ");
                    }
                }
                else
                {
                    _logger.LogWarning($"El formato para login para {request.Login} no corresponde a usuario local ni a usuario AD");
                }

                if(usuarioAutenticado == null && idUsuarioPToken == Guid.Empty.ToString())
                {
                    return null; //Autenticacion fallida//talvez se quiera contar los intentos
                }

                var token = GenerarJwtToken(idUsuarioPToken, request.Login, rolAutenticacion);

                return new LoginResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Rol = rolAutenticacion,
                    FechaExpiracion = token.ValidTo
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"error durante el proceso de autenticacion {request.Login}");
                throw;
            }

        }


        private async Task<bool> AutenticarConLdapAsync(string nombreCuenta, string password)
        {
            LdapConnection? connection = null;
            try
            {
                connection = new LdapConnection(new LdapDirectoryIdentifier(_ldapOptions.DirectoryEntry, 389));//puerto de servidor utilizado 
                connection.AuthType = AuthType.Negotiate;

                connection.Credential = new NetworkCredential(nombreCuenta, password, _ldapOptions.Domain);

                await Task.Run(() => connection.Bind()); //conexion exitosa
                return true;
            }
            catch (LdapException ex)
            {
                _logger.LogWarning($"Autenticacion LDAP Fallida para {nombreCuenta}. Error: {ex.Message}");
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error inesperado durante la autenticacion LDAP para {nombreCuenta}");
                return false;
            }
            finally
            {
                connection?.Dispose();
            }

        }

        private async Task<bool> AutenticarJson(string nombreCuenta, string password)
        {
            ServicioUsuarioLdap usuarioLdap;
            try
            {
                usuarioLdap = new ServicioUsuarioLdap();
                return await usuarioLdap.ExisteUsuarioAsync(nombreCuenta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al autenticar usuario JSON: {nombreCuenta}");
                return false;
            }

        }

        private JwtSecurityToken GenerarJwtToken(string userId, string nombreUsuario, string rol)
        {
            var jwtSecreto = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtSecreto))
            {
                _logger.LogError("La clave secreta JWT No esta configurada en appsettings.json");
                throw new InvalidOperationException($"La Clave JWT no esta configurada");
            }

            var claveSeguridad = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecreto));
            var credenciales = new SigningCredentials(claveSeguridad, SecurityAlgorithms.HmacSha256);

            var declaraciones = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nombreUsuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, nombreUsuario),
                new Claim(ClaimTypes.Role, rol)
            };

            var minutosExpiracion = Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]);
            var expiraEn = DateTime.UtcNow.AddMinutes(minutosExpiracion);


            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: declaraciones,
                expires: expiraEn,
                signingCredentials: credenciales);
            return token;

        }


    }
}

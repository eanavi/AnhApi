using System.Text.Json;
using AnhApi.Esquemas;
using AnhApi.Modelos;


/**
 * Servicio para verificar la existencia de usuarios en un archivo JSON.
 * Este servicio se utiliza envez de la busqueda directa en el servidor de Dominio, en produccion se hara el cambio y pruebas respectivas
 */
namespace AnhApi.Servicios
{
    public class ServicioUsuarioLdap
    {
        private readonly string _rutaArchivo = Path.Combine("Datos", "usuariosldap.json");
        private readonly string _dominio = "@anh.gob.bo";


        public async Task<bool> ExisteUsuarioAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return false;

            string loginCompleto = nombreUsuario.Trim().ToUpper();

            if (!nombreUsuario.Contains("@anh.gob.bo", StringComparison.OrdinalIgnoreCase))
                loginCompleto = $"{nombreUsuario.Trim().ToUpper()}{_dominio}";


            if (!File.Exists(_rutaArchivo))
                return false;

            var contenidoJson = await File.ReadAllTextAsync(_rutaArchivo);
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var usuarios = JsonSerializer.Deserialize<List<UsuarioLdap>>(contenidoJson, opciones);

            return usuarios?.Any(u => u.usuariO_DOMINIO?.Equals(loginCompleto, StringComparison.OrdinalIgnoreCase) == true) ?? false;
        }
    }
}

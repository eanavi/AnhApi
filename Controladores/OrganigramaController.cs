using AnhApi.Esquemas;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrganigramaController : ControllerBase
    {
        private readonly ILogger<OrganigramaController> _logger;
        private readonly Interfaces.IServicioOrganigrama _servicioOrganigrama;
        private readonly IMapper _mapper;
        public OrganigramaController(ILogger<OrganigramaController> logger, Interfaces.IServicioOrganigrama servicioOrganigrama, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _servicioOrganigrama = servicioOrganigrama;
            _mapper = mapper;
        }

        private Dictionary<int, OrganigramaNodoDto> ConstruirDiccionario(IEnumerable<Organigrama> organigramas)
        {
            return organigramas.ToDictionary(o => o.id_organigrama, o => new OrganigramaNodoDto
            {
                IdOrganigrama = o.id_organigrama,
                NombreOrganigrama = o.nombre_organigrama,
                Sigla = o.sigla
            });
        }

        private List<OrganigramaNodoDto> ConstruirJerarquia(IEnumerable<Organigrama> organigramas, Dictionary<int, OrganigramaNodoDto> diccionario)
        {
            List<OrganigramaNodoDto> raiz = new();
            foreach (var org in organigramas)
            {
                if (org.id_organigrama_padre.HasValue)
                {
                    diccionario[org.id_organigrama_padre.Value].Hijos.Add(diccionario[org.id_organigrama]);
                }
                else
                {
                    raiz.Add(diccionario[org.id_organigrama]);
                }
            }

            return raiz;

        }

        // Aquí puedes agregar métodos para manejar las solicitudes relacionadas con el organigrama.


        [HttpGet]
        [ProducesResponseType(typeof(OrganigramaNodoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<OrganigramaNodoDto>>> GetArbol()
        {
            try
            {
                var organigramas = await _servicioOrganigrama.ObtenerTodosAsync();

                var diccionario = ConstruirDiccionario(organigramas);

                var arbol = ConstruirJerarquia(organigramas, diccionario);

                return Ok(arbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las unidades");
                return StatusCode(500, $"Error Interno del servidor");
            }

        }



        [HttpGet("rama/{id}")]
        [ProducesResponseType(typeof(OrganigramaNodoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<OrganigramaNodoDto>>> GetRama(int id)
        {
            try
            {
                var organigramas = await _servicioOrganigrama.ObtenerTodosAsync();

                var diccionario = ConstruirDiccionario(organigramas);
                var arbol = ConstruirJerarquia(organigramas, diccionario);

                if (!diccionario.ContainsKey(id))
                {
                    return NotFound($"No Existe Organigrama con id {id}");
                }

                return Ok(diccionario[id]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las unidades");
                return StatusCode(500, $"Error Interno del servidor");
            }
        }


        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EsqOrganigrama), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqOrganigrama>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var organigrama = await _servicioOrganigrama.ObtenerPorIdAsync(id);
                if (organigrama == null)
                {
                    return NotFound($"no se encontro la unidad organizacional requerida con el id {id}");
                }

                var esqOrg = _mapper.Map<EsqOrganigrama>(organigrama);
                return Ok(esqOrg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el municipio con Id {id}");
                return StatusCode(500, "Error interno del servidor al obtener la unidad");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(EsqOrganigrama), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqOrganigrama>> CrearOrganigrama([FromBody] OrganigramaCreacion orgC)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var org = _mapper.Map<Organigrama>(orgC);
                string usuario = User.Identity?.Name ?? "sistema_api";//Usuaro que realiza la accion
                string ipAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
                var nuevoOrg = await _servicioOrganigrama.CrearAsync(org, usuario, ipAuditoria);

                var esqOrg = _mapper.Map<Organigrama>(nuevoOrg);

                return CreatedAtAction(nameof(ObtenerPorIdAsync), new { id = esqOrg.id_organigrama }, esqOrg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva persona.");
                return StatusCode(500, "Error interno del servidor al crear la unidad");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarOrganigrama([FromRoute] int id, [FromBody] EsqOrganigrama organigrama)
        {
            try
            {
                if (id != organigrama.IdOrganigrama)
                {
                    return BadRequest("El Id de la ruta no coincide con el ID del cuerpo de la solicitud");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var org = _mapper.Map<Organigrama>(organigrama);

                string usuario = User.Identity?.Name ?? "sistema_api";
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
                var resultado = await _servicioOrganigrama.ActualizarAsync(id, org, usuario, ip);

                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo actualizar la unidad con Id {id}");
                    return NotFound($"Persona con Id {id} no encontrada o no se pudo actualizar");
                }

                return NoContent();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la unidad con id  {id} ");
                return StatusCode(500, $"Error interno del servidor al actualizar la persona con Id {id}");
            }

        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EliminarOrganigrama([FromRoute] int id)
        {
            try
            {
                string usuario = User.Identity?.Name ?? "siis_api";
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";

                var resultado = await _servicioOrganigrama.EliminarAsync(id, usuario, ip);

                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo eliminar la unidad con id {id}");
                    return NotFound($"Unidad con id{id} no encontrada ");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la unidad con id {id}");
                return StatusCode(500, $"error Interno ");
            }
        }
    }
}
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


namespace AnhApi.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartamentoController : ControllerBase
    {
        private readonly IServicioDepartamento _servDepartamento;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartamentoController> _logger;

        public DepartamentoController(
            IServicioDepartamento servDepartamento,
            IMapper mapper,
            ILogger<DepartamentoController> logger)
        {
            _servDepartamento = servDepartamento ?? throw new ArgumentNullException(nameof(servDepartamento));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginacionResultado<DepartamentoListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EnableRateLimiting("fijo")]// Limita la tasa de solicitudes a este endpoint
        [ProducesResponseType(429)]// Demasiadas peticiones
        [ProducesResponseType(400)]//Paginacion erronea
        [ProducesResponseType(401)]//No autorizado
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginacionResultado<DepartamentoListado>>> ObtenerTodosPagAsync([FromQuery] PaginacionParametros paginacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var departamentosPag = await _servDepartamento.ObtenerTodosPagAsync(paginacion);
                if (departamentosPag == null)
                {
                    return NotFound("No se encontraron departamentos.");
                }
                var departamentosLista = _mapper.Map<IEnumerable<DepartamentoListado>>(departamentosPag.Elementos);
                var resultado = new PaginacionResultado<DepartamentoListado>
                {
                    Elementos = departamentosLista,
                    TotalRegistros = departamentosPag.TotalRegistros,
                    PaginaActual = departamentosPag.PaginaActual,
                    TamanoPagina = departamentosPag.TamanoPagina,
                    TotalPaginas = (int)Math.Ceiling((double)departamentosPag.TotalRegistros / paginacion.TamanoPagina)
                };
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los departamentos paginados.");
                return StatusCode(500, "Error interno del servidor al obtener los departamentos.");
            }


        }

        /// <summary>
        /// Buscar paises paginados por un criterio de busqueda.
        /// </summary>
        /// <param name="criterio">Puede ser el nombre de un pais, codigo 2 letras, codigo 3 letras o cod numerico</param>
        /// <param name="paginacion">PaginaNumero=...&TamanoPagina..</param>
        /// <returns></returns>
        [HttpGet("buscar/{criterio:alpha}")]
        [ProducesResponseType(typeof(IEnumerable<DepartamentoListado>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DepartamentoListado>>> Buscar([FromRoute] string? criterio)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var resultadoPag = await _servDepartamento.Buscar(criterio);

                return Ok(resultadoPag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al buscar países paginados con criterio '{criterio}'.");
                return StatusCode(500, "Error interno del servidor al buscar los países.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EsqDepartamento), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqDepartamento>> ObtenerPorId([FromRoute] int id)
        {
            try
            {
                var departamento = await _servDepartamento.ObtenerPorIdAsync(id);
                if (departamento == null)
                {
                    return NotFound($"No se encontró el departamento con ID {id}.");
                }

                var departamentoEsq = _mapper.Map<EsqDepartamento>(departamento);
                return Ok(departamentoEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el departamento con ID {id}.");
                return StatusCode(500, "Error interno del servidor al obtener el departamento.");
            }
        }

        [HttpGet("provincias/{id:int}")]
        [ProducesResponseType(typeof(DeptoConProvinciasEsq), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DeptoConProvinciasEsq>> ObtenerDeptConProvincias([FromRoute] int id)
        {
            try
            {
                var deptoConProv = await _servDepartamento.ObtenerDeptoConProvinciasAsync(id);
                if (deptoConProv == null)
                {
                    return NotFound($"no se encontro un departamento cn el id {id}");
                }
                var deptoConPEsq = _mapper.Map<DeptoConProvinciasEsq>(deptoConProv);
                return Ok(deptoConPEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el departamento con id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "error Interno");
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(EsqDepartamento), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EsqDepartamento>> CrearDepartamento([FromBody] DepartamentoCreacion deptoCreacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var departamento = _mapper.Map<Departamento>(deptoCreacion);
                departamento.aud_estado = 0; // Estado activo por defecto
                var creado = await _servDepartamento.CrearAsync(departamento);
                var departamentoEsq = _mapper.Map<EsqDepartamento>(creado);

                return CreatedAtAction(nameof(ObtenerPorId), new { id = departamento.id_departamento }, departamentoEsq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el departamento.");
                return StatusCode(500, "Error interno del servidor al crear el departamento.");
            }

        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarDepartamento([FromRoute] int id, [FromBody] EsqDepartamento depto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var deptoMod = _mapper.Map<Departamento>(depto);
                var resultado = await _servDepartamento.ActualizarAsync(id, deptoMod);
                if (!resultado)
                {
                    _logger.LogInformation($"No se pudo actualizar el departamento con ID {id}.");
                    return NotFound($"Departamento con ID {id} no encontrado.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el departamento con ID {id}.");
                return StatusCode(500, "Error interno del servidor al actualizar el departamento.");
            }
        }
    }
}

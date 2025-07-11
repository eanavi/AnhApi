using EsqPersona = AnhApi.Esquemas.Persona;
using AnhApi.Modelos;
using AnhApi.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json;

namespace AnhApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonasController : ControllerBase
    {
        private readonly IGenericoServicio<Persona, EsqPersona, Guid> _personaServicio;

        public PersonasController(IGenericoServicio<Persona, EsqPersona, Guid> personaServicio)
        {
            _personaServicio = personaServicio;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EsqPersona>>> GetAll()
        {
            var personas = await _personaServicio.ObtenerTodosAsync();
            return Ok(personas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EsqPersona>> GetById(Guid id)
        {
            var persona = await _personaServicio.ObtenerPorIdAsync(id);
            if (persona == null) return NotFound();
            return Ok(persona);
        }


        [HttpPost]
        public async Task<ActionResult<EsqPersona>> Create(EsqPersona personaDto)
        {
            if (personaDto == null)
            {
                return BadRequest("El objeto persona no puede ser nulo.");
            }

            string? direccionJson = null;

            if (personaDto.Direccion != null)
            {
                try
                {
                    direccionJson = JsonSerializer.Serialize(personaDto.Direccion);
                    JsonDocument.Parse(direccionJson);
                }
                catch (JsonException)
                {
                    return BadRequest("La dirección debe ser un JSON válido.");
                }
            }

            personaDto.Direccion = direccionJson != null ? JsonDocument.Parse(direccionJson) : null;

            string? telefonoJson = null;

            if (personaDto.Telefono != null)
            {
                try
                {
                    telefonoJson = JsonSerializer.Serialize(personaDto.Telefono);
                    JsonDocument.Parse(telefonoJson);
                }
                catch (JsonException)
                {
                    return BadRequest("El teléfono debe ser un JSON válido.");
                }
            }

            personaDto.Telefono = telefonoJson != null ? JsonDocument.Parse(telefonoJson) : null;

            string? correoJson = null;
            if (personaDto.Correo != null)
            {
                try
                {
                    correoJson = JsonSerializer.Serialize(personaDto.Correo);
                    JsonDocument.Parse(correoJson);
                }
                catch (JsonException)
                {
                    return BadRequest("El correo debe ser un JSON válido.");
                }
            }

            personaDto.Correo = correoJson != null ? JsonDocument.Parse(correoJson) : null;

            personaDto.FechaNacimiento = personaDto.FechaNacimiento.ToUniversalTime();

            var persona = await _personaServicio.CrearAsync(personaDto);
            return CreatedAtAction(nameof(GetById), new { id = persona.IdPersona }, persona);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody]EsqPersona personaDto)
        {
            
            if (personaDto == null)
            {
                return BadRequest("El objeto persona no puede ser nulo.");
            }

            string? direccionJson = null;

            if (personaDto.Direccion != null)
            {
                try
                {
                    direccionJson = JsonSerializer.Serialize(personaDto.Direccion);
                    JsonDocument.Parse(direccionJson);
                }
                catch (JsonException)
                {
                    return BadRequest("La dirección debe ser un JSON válido.");
                }
            }

            personaDto.Direccion = direccionJson != null ? JsonDocument.Parse(direccionJson) : null;

            string? telefonoJson = null;

            if (personaDto.Telefono != null)
            {
                try
                {
                    telefonoJson = JsonSerializer.Serialize(personaDto.Telefono);
                    JsonDocument.Parse(telefonoJson);
                }
                catch (JsonException)
                {
                    return BadRequest("El teléfono debe ser un JSON válido.");
                }
            }

            personaDto.Telefono = telefonoJson != null ? JsonDocument.Parse(telefonoJson) : null;

            string? correoJson = null;
            if (personaDto.Correo != null)
            {
                try
                {
                    correoJson = JsonSerializer.Serialize(personaDto.Correo);
                    JsonDocument.Parse(correoJson);
                }
                catch (JsonException)
                {
                    return BadRequest("El correo debe ser un JSON válido.");
                }
            }

            personaDto.Correo = correoJson != null ? JsonDocument.Parse(correoJson) : null;

            personaDto.FechaNacimiento = personaDto.FechaNacimiento.ToUniversalTime();

            var result = await _personaServicio.ActualizarAsync(personaDto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _personaServicio.EliminarAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("fisico/{id}")]
        public async Task<ActionResult<EsqPersona>> DeletePhysical(Guid id)
        {
            var persona = await _personaServicio.EliminarFisicoAsync(id);
            if (persona == null) return NotFound();
            return Ok(persona);
        }
    }
}
using AnhApi.Datos;
using EsqPersona = AnhApi.Esquemas.Persona;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class PersonaServicio : GenericoServicio<Persona, EsqPersona, Guid>
    {
        public PersonaServicio(AppDbContext context, IMapper mapper) : base(context, mapper)
        {
            // Métodos adicionales específicos para Persona, si los necesitas
        }

  

        // Ejemplo de método adicional
        public async Task<IEnumerable<EsqPersona>> BuscarPorNombreAsync(string nombre)
        {
            var entidades = await _context.Set<Persona>()
                .Where(p => p.nombre.Contains(nombre) && p.aud_estado == 0)
                .ToListAsync();
            return _mapper.Map<IEnumerable<EsqPersona>>(entidades);
        }
    }
}
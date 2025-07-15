using AnhApi.Datos;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnhApi.Servicios
{
    // PersonaServicio ahora hereda de la versión corregida de GenericoServicio
    public class PersonaServicio : GenericoServicio<Persona, Guid>
    {
        // Inyectamos IMapper directamente en esta clase, no en la base
        private readonly IMapper _mapper;

        public PersonaServicio(AppDbContext context, IMapper mapper,ILogger<GenericoServicio<Persona, Guid>> logger) : base(context, logger)
        {
            _mapper = mapper;
        }

        // --- MÉTODOS QUE DEVUELVEN MODELOS (Recomendado) ---
        // Este método se encargará de la lógica de negocio y devolverá el modelo
        public async Task<IEnumerable<Persona>> BuscarPorNombreAsync(string nombre)
        {
            var entidades = await _context.Set<Persona>()
                .Where(p => p.nombre.Contains(nombre) && p.aud_estado == 0)
                .ToListAsync();

            return entidades; // Devuelve la lista de modelos
        }

        // --- MÉTODOS QUE DEVUELVEN DTOS (Alternativa si lo necesitas) ---
        // En este caso el servicio mapea a un DTO.
        // public async Task<IEnumerable<EsqPersona>> BuscarPorNombreComoDtoAsync(string nombre)
        // {
        //     var entidades = await _context.Set<Persona>()
        //         .Where(p => p.nombre.Contains(nombre) && p.aud_estado == 0)
        //         .ToListAsync();
        //     return _mapper.Map<IEnumerable<EsqPersona>>(entidades); // Mapea a DTO
        // }
    }
}
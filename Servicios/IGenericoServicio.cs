using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnhApi.Servicios
{
    public interface IGenericoServicio<TEntity, TDto, TId> where TEntity : class where TDto : class
    {
        Task<IEnumerable<TDto>> ObtenerTodosAsync();
        Task<TDto?> ObtenerPorIdAsync(TId id);
        Task<TDto> CrearAsync(TDto dto);
        Task<bool> ActualizarAsync(TDto dto);
        Task<bool> EliminarAsync(TId id);
        Task<TDto?> EliminarFisicoAsync(TId id);
    }
}
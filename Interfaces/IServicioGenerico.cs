using AnhApi.Esquemas;

namespace AnhApi.Interfaces
{
    public interface IServicioGenerico<T, TKey>
    {
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<PaginacionResultado<T>> ObtenerTodosPagAsync(PaginacionParametros parametros);
        Task<T> ObtenerPorIdAsync(TKey id);
        Task<T> CrearAsync(T entidad);
        Task<bool> ActualizarAsync(TKey id, T entidad);
        Task<bool> EliminarAsync(TKey id);
        Task<T?> EliminarFisicoAsync(TKey id);

    }
}
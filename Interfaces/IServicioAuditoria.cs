using AnhApi.Esquemas;

namespace AnhApi.Interfaces
{
    public interface IServicioAuditoria<T, TKey>
    {
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<PaginacionResultado<T>> ObtenerTodosPagAsync(PaginacionParametros parametros);
        Task<T> ObtenerPorIdAsync(TKey id);
        Task<T> CrearAsync(T entidad, string usuario, string ip);
        Task<bool> ActualizarAsync(TKey id, T entidad, string usuario, string ip);
        Task<bool> EliminarAsync(TKey id, string usuario, string ip);
        Task<T?> EliminarFisicoAsync(TKey id);
    }
}

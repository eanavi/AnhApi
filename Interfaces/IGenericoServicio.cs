namespace AnhApi.Interfaces
{
    public interface IGenericoServicio<T, TKey>
    {
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<T> ObtenerPorIdAsync(TKey id);
        Task<T> CrearAsync(T entidad, string usuario, string ip);
        Task<bool> ActualizarAsync(TKey id, T entidad, string usuario, string ip);
        Task<bool> EliminarAsync(TKey id, string usuario, string ip);
        Task<T?> EliminarFisicoAsync(TKey id);
    }
}

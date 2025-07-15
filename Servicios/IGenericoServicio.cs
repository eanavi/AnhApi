// Antes (la definición de tu interfaz)
// public interface IGenericoServicio<T, TDto, TId>

// Después (la definición CORREGIDA de tu interfaz)
using AnhApi.Modelos;

public interface IGenericoServicio<T, TId>
    where T : ModeloBase
{
    Task<IEnumerable<T>> ObtenerTodosAsync();
    Task<T?> ObtenerPorIdAsync(TId id);
    Task<T> CrearAsync(T entidad, string usuario, string ip);
    Task<bool> ActualizarAsync(TId id, T entidad, string usuario, string ip);
    Task<bool> EliminarAsync(TId id, string usuario, string ip);
    Task<T?> EliminarFisicoAsync(TId id);
}
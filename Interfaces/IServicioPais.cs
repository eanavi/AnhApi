using AnhApi.Modelos.prm;

namespace AnhApi.Interfaces
{
    public interface IServicioPais : IServicioGenerico<Pais, int>
    {

        Task<IEnumerable<Pais>> Buscar(string criterio);
        Task<Pais?> ObtenerPaisConDepartamentosAsync(int id);
    }
}

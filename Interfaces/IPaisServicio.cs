using AnhApi.Modelos.prm;

namespace AnhApi.Interfaces
{
    public interface IPaisServicio : IGenericoServicio<Pais, int>
    {

        Task<IEnumerable<Pais>> Buscar(string criterio);
    }
}

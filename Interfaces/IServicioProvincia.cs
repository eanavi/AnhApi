using AnhApi.Modelos.prm;

namespace AnhApi.Interfaces
{
    public interface IServicioProvincia : IServicioGenerico<Provincia, int>
    {
        Task<IEnumerable<Provincia>> Buscar(string criterio);
        Task<Provincia?> ObtenerProvinciaConMunicipiosAsync(int id);
        Task<Provincia?> ObtenerProvinciaConLocalidadesAsync(int id);
    }
}

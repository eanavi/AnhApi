using AnhApi.Modelos.prm;
namespace AnhApi.Interfaces
{
    public interface IServicioMunicipio : IServicioGenerico<Municipio, int>
    {
        Task<IEnumerable<Municipio>> Buscar(string criterio);
        Task<IEnumerable<Jerarquia>> BuscarJerarquia(string criterio);
    }
}

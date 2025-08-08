using AnhApi.Modelos.prm;


namespace AnhApi.Interfaces
{
    public interface IServicioLocalidad : IServicioGenerico<Localidad, int>
    {
        Task<IEnumerable<Localidad>> Buscar(string criterio);
        Task<IEnumerable<Jerarquia>> BuscarJerarquia(string criterio);
    }
}

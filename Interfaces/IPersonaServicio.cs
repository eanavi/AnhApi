using AnhApi.Esquemas;
using AnhApi.Modelos;

namespace AnhApi.Interfaces
{
    public interface IPersonaServicio : IGenericoServicio<Persona, Guid>
    {
        Task<IEnumerable<Persona>> Buscar(string criterio);
        Task<PaginacionResultado<PersonaListado>> BuscarPaginado(string criterio, PaginacionParametros paginacion);

    }
}

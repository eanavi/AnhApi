using AnhApi.Esquemas;
using AnhApi.Modelos;

namespace AnhApi.Interfaces
{
    public interface IPersonaServicio : IAuditoriaServicio<Persona, Guid>
    {
        Task<IEnumerable<Persona>> Buscar(string criterio);
        Task<PaginacionResultado<PersonaListado>> BuscarPaginado(string criterio, PaginacionParametros paginacion);

    }
}

using AnhApi.Esquemas;
using AnhApi.Modelos;

namespace AnhApi.Interfaces
{
    public interface IServicioPersona : IServicioAuditoria<Persona, Guid>
    {
        Task<IEnumerable<Persona>> Buscar(string criterio);
        Task<PaginacionResultado<PersonaListado>> BuscarPaginado(string criterio, PaginacionParametros paginacion);

    }
}

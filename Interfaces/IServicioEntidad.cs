using AnhApi.Esquemas;
using AnhApi.Modelos;

namespace AnhApi.Interfaces
{
    public interface IServicioEntidad : IServicioAuditoria<Entidad, Guid>
    {
        Task<PaginacionResultado<EntidadListado>> listarPaginado(PaginacionParametros paginacion);
        Task<PaginacionResultado<EntidadListado>> BuscarPaginado(string criterio, PaginacionParametros paginacion);
        Task<EntidadListadoRepresentante> EntidadConRepresentante(Guid id);
        Task<EntidadListadoDocumentos> EntidadDocumentos(Guid id);
    }
}

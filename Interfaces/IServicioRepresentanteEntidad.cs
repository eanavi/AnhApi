using AnhApi.Esquemas;
using AnhApi.Modelos;

namespace AnhApi.Interfaces
{
    public interface IServicioRepresentanteEntidad : IServicioAuditoria<RepresentanteEntidad, int>
    {
        Task<IEnumerable<EsqRepresentanteEntidad>> Listar(Guid idEntidad);
    }
}
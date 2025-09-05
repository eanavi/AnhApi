using AnhApi.Esquemas;
using AnhApi.Modelos;


namespace AnhApi.Interfaces
{
    public interface IServicioDocumentoEntidad : IServicioAuditoria<DocumentoEntidad, int>
    {
        Task<ICollection<DocEntidadDespliegue>> ObtenerPorEntidadIdAsync(Guid idEntidad);
    }
}

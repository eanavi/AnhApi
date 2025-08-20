using AnhApi.Modelos;
using AnhApi.Esquemas;


namespace AnhApi.Interfaces
{
    public interface IServicioDocumentoEntidad : IServicioAuditoria<DocumentoEntidad, int>
    {
        Task<ICollection<DocEntidadDespliegue>> ObtenerPorEntidadIdAsync(Guid idEntidad);
    }
}

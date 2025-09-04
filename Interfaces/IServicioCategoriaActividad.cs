using AnhApi.Esquemas;
using AnhApi.Modelos;

namespace AnhApi.Interfaces
{
    public interface IServicioCategoriaActividad : IServicioAuditoria<CategoriaActividad, int>
    {
        Task<IEnumerable<CategoriaActividadListado>> ListadoCategoria();
    }
}

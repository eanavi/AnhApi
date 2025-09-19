using AnhApi.Modelos;

namespace AnhApi.Interfaces
{
    public interface IServicioOperacion : IServicioAuditoria<Operacion, int>
    {
        Task<IEnumerable<Operacion>> Buscar(string criterio);

    }
}

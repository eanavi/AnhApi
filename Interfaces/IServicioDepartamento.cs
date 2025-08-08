using AnhApi.Modelos;
using AnhApi.Modelos.prm;

namespace AnhApi.Interfaces
{
    public interface IServicioDepartamento : IServicioGenerico<Departamento, int>
    {
        Task<IEnumerable<Departamento>> Buscar(string criterio);
        Task<Departamento?> ObtenerDeptoConProvinciasAsync(int id); 
    }
}

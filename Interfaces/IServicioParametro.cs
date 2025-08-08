using AnhApi.Modelos.prm;

namespace AnhApi.Interfaces
{
    public interface IServicioParametro : IServicioGenerico<Parametro, int>
    {
        //Metodos especificos
        Task<IEnumerable<Parametro>> ObtenerPorGrupo(string grupo);
    }
}

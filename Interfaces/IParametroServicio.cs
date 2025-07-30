using AnhApi.Modelos.prm;

namespace AnhApi.Interfaces
{
    public interface IParametroServicio : IGenericoServicio<Parametro, int>
    {
        //Metodos especificos
        Task<IEnumerable<Parametro>> ObtenerPorGrupo(string grupo);
    }
}

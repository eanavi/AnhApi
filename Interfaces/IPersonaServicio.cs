using AnhApi.Modelos;

namespace AnhApi.Interfaces
{
    public interface IPersonaServicio : IGenericoServicio<Persona, Guid>
    {
        Task<IEnumerable<Persona>> Buscar(string criterio);
    }
}

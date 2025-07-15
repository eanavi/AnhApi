using AnhApi.Datos;
using AnhApi.Modelos;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace AnhApi.Servicios
{
    public class UsuarioServicio : GenericoServicio<Modelos.Usuario, Guid>
    {
        public UsuarioServicio(AppDbContext context, ILogger<GenericoServicio<Modelos.Usuario, Guid>> logger,IMapper mapper ) 
            : base(context, logger)
        {
        }

    }
}

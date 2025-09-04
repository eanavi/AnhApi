using AnhApi.Datos;
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class ServicioRepresentanteEntidad : ServicioAuditoria<RepresentanteEntidad, int>, IServicioRepresentanteEntidad
    {
        private readonly ContextoAppBD _contexto;
        private readonly ILogger<ServicioRepresentanteEntidad> _logger;
        private readonly IMapper _mapper;

        public ServicioRepresentanteEntidad(ContextoAppBD contexto, ILogger<ServicioRepresentanteEntidad> logger, IMapper mapper) 
            : base(contexto, logger)
        {
            _contexto = contexto;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EsqRepresentanteEntidad>> Listar(Guid idEntidad)
        {
            try
            {
                if (idEntidad == Guid.Empty)
                {
                    _logger.LogWarning("El id de la entidad no puede ser vacío.");
                    throw new ArgumentException("El id de la entidad no puede ser vacío.", nameof(idEntidad));
                }

                var representantes = await _contexto.RepresentantesEntidad
                    .Include(r => r.Persona)
                    .Where(r => r.id_entidad == idEntidad && r.aud_estado == 0)
                    .ToListAsync();

                if (representantes == null || !representantes.Any())
                {
                    _logger.LogInformation("No se encontraron representantes para la entidad con id {IdEntidad}", idEntidad);
                    return Enumerable.Empty<EsqRepresentanteEntidad>();
                }

                var parametros = await _contexto.Parametros
                    .Where(p => p.grupo == "tipo_representante")
                    .ToListAsync();

                var representantesDto = representantes.Select(r => new EsqRepresentanteEntidad
                {
                    IdRepresentanteEntidad = r.id_representante_entidad,
                    TipoRepresentante = parametros.FirstOrDefault(p => p.codigo == r.tipo_representante)?.descripcion ?? "Desconocido",
                    IdPersona = r.id_persona,
                    IdEntidad = r.id_entidad,
                    NombreRepresentante = $"{r.Persona.nombre} {r.Persona.primer_apellido} {r.Persona.segundo_apellido}".Trim(),
                    Ci = r.Persona.numero_identificacion.Trim()
                });

                return representantesDto;

            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error al validar el id de la entidad");
                throw;
            }
        }

    }
}

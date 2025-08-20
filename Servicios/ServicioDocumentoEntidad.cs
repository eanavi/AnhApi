using AnhApi.Datos;
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using AnhApi.Modelos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class ServicioDocumentoEntidad : ServicioAuditoria<DocumentoEntidad, int>, IServicioDocumentoEntidad
    {
        private readonly ContextoAppBD _contextoBd;
        private readonly ILogger<ServicioDocumentoEntidad> _logger;
        private readonly IMapper _mapper;
        public ServicioDocumentoEntidad(ContextoAppBD contextoBd, ILogger<ServicioDocumentoEntidad> logger, IMapper mapper)
            : base(contextoBd, logger)
        {
            _contextoBd = contextoBd ?? throw new ArgumentNullException(nameof(contextoBd));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper;
        }

        public async Task<ICollection<DocEntidadDespliegue>> ObtenerPorEntidadIdAsync(Guid idEntidad)
        {
            try
            {
                if (idEntidad == Guid.Empty)
                {
                    throw new ArgumentException("El ID de la entidad no puede estar vacío.", nameof(idEntidad));
                }

                var documentos = await _contextoBd.DocumentosEntidad
                    .Where(d => d.id_entidad == idEntidad)
                    .OrderBy(d => d.id_documento_entidad)
                    .Select(d => new DocEntidadDespliegue
                    {
                        IdDocumentoEntidad = d.id_documento_entidad,
                        IdEntidad = d.id_entidad,
                        TipoDocInscr = d.tipo_doc_inscr,
                        DescripcionTipoDoc = _contextoBd.Parametros
                            .Where(p => p.grupo == "tipo_doc_inscr" && p.codigo == d.tipo_doc_inscr)
                            .Select(p => p.descripcion)
                            .FirstOrDefault(),
                        Cite = d.cite,
                        FechaDoc = d.fecha_doc,
                        NombreArchivo = d.nombre_archivo,
                        UrlArchivo = d.url_archivo
                    })
                    .ToListAsync();
                return documentos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener documentos de entidad por ID");
                throw new ApplicationException($"Error al obtener documentos de entidad con ID {idEntidad}", ex);
            }
        }
    }
}

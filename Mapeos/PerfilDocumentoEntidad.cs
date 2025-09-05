// Archivo: AnhApi.Mapeos/DocumentoEntidadPerfil.cs
using AnhApi.Esquemas;
using AnhApi.Modelos;
using AutoMapper;

namespace AnhApi.Mapeos
{
    public class PerfilDocumentoEntidad : Profile
    {
        public PerfilDocumentoEntidad()
        {
            // Mapeo de DTO de Creación a Modelo de Dominio
            CreateMap<DocumentoEntidadCreacion, DocumentoEntidad>()
                .ForMember(dest => dest.id_documento_entidad, opt => opt.Ignore()) // ID es autoincremental
                .ForMember(dest => dest.id_entidad, opt => opt.MapFrom(src => src.IdEntidad))
                .ForMember(dest => dest.tipo_doc_inscr, opt => opt.MapFrom(src => src.TipoDocInscr))
                .ForMember(dest => dest.cite, opt => opt.MapFrom(src => src.Cite))
                .ForMember(dest => dest.fecha_doc, opt => opt.MapFrom(src => src.FechaDoc))
                .ForMember(dest => dest.nombre_archivo, opt => opt.MapFrom(src => src.NombreArchivo))
                .ForMember(dest => dest.url_archivo, opt => opt.MapFrom(src => src.UrlArchivo))
                .ForMember(dest => dest.observaciones, opt => opt.MapFrom(src => src.Observaciones))
                // Ignorar campos de auditoría, se manejan en el servicio
                .ForMember(dest => dest.aud_estado, opt => opt.Ignore())
                .ForMember(dest => dest.aud_usuario, opt => opt.Ignore())
                .ForMember(dest => dest.aud_fecha, opt => opt.Ignore())
                .ForMember(dest => dest.aud_ip, opt => opt.Ignore());

            // Mapeo de Modelo de Dominio a DTO de Listado
            CreateMap<DocumentoEntidad, DocumentoEntidadListado>()
                .ForMember(dest => dest.IdDocumentoEntidad, opt => opt.MapFrom(src => src.id_documento_entidad))
                .ForMember(dest => dest.IdEntidad, opt => opt.MapFrom(src => src.id_entidad))
                .ForMember(dest => dest.TipoDocInscr, opt => opt.MapFrom(src => src.tipo_doc_inscr))
                .ForMember(dest => dest.Cite, opt => opt.MapFrom(src => src.cite))
                .ForMember(dest => dest.FechaDoc, opt => opt.MapFrom(src => src.fecha_doc))
                .ForMember(dest => dest.NombreArchivo, opt => opt.MapFrom(src => src.nombre_archivo))
                .ForMember(dest => dest.UrlArchivo, opt => opt.MapFrom(src => src.url_archivo));

            // Mapeo de Modelo de Dominio a DTO de Esquema Completo
            CreateMap<DocumentoEntidad, EsqDocumentoEntidad>()
                .ForMember(dest => dest.IdDocumentoEntidad, opt => opt.MapFrom(src => src.id_documento_entidad))
                .ForMember(dest => dest.IdEntidad, opt => opt.MapFrom(src => src.id_entidad))
                .ForMember(dest => dest.TipoDocInscr, opt => opt.MapFrom(src => src.tipo_doc_inscr))
                .ForMember(dest => dest.Cite, opt => opt.MapFrom(src => src.cite))
                .ForMember(dest => dest.FechaDoc, opt => opt.MapFrom(src => src.fecha_doc))
                .ForMember(dest => dest.NombreArchivo, opt => opt.MapFrom(src => src.nombre_archivo))
                .ForMember(dest => dest.UrlArchivo, opt => opt.MapFrom(src => src.url_archivo))
                .ForMember(dest => dest.Observaciones, opt => opt.MapFrom(src => src.observaciones))
                // Mapear campos de auditoría
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.AudUsuario, opt => opt.MapFrom(src => src.aud_usuario))
                .ForMember(dest => dest.AudFecha, opt => opt.MapFrom(src => src.aud_fecha))
                .ForMember(dest => dest.AudIp, opt => opt.MapFrom(src => src.aud_ip));

            // Mapeo de DTO de Esquema Completo a Modelo de Dominio (para Actualización)
            CreateMap<EsqDocumentoEntidad, DocumentoEntidad>()
                .ForMember(dest => dest.id_documento_entidad, opt => opt.MapFrom(src => src.IdDocumentoEntidad))
                .ForMember(dest => dest.id_entidad, opt => opt.MapFrom(src => src.IdEntidad))
                .ForMember(dest => dest.tipo_doc_inscr, opt => opt.MapFrom(src => src.TipoDocInscr))
                .ForMember(dest => dest.cite, opt => opt.MapFrom(src => src.Cite))
                .ForMember(dest => dest.fecha_doc, opt => opt.MapFrom(src => src.FechaDoc))
                .ForMember(dest => dest.nombre_archivo, opt => opt.MapFrom(src => src.NombreArchivo))
                .ForMember(dest => dest.url_archivo, opt => opt.MapFrom(src => src.UrlArchivo))
                .ForMember(dest => dest.observaciones, opt => opt.MapFrom(src => src.Observaciones))
                // Mapear campos de auditoría desde el DTO al modelo para la actualización
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado))
                // Los campos aud_usuario, aud_ip, aud_fecha suelen ser gestionados por el servicio en la actualización
                .ForMember(dest => dest.aud_usuario, opt => opt.Ignore()) // Opcional: Ignorar si el servicio los asigna
                .ForMember(dest => dest.aud_ip, opt => opt.Ignore())     // Opcional: Ignorar si el servicio los asigna
                .ForMember(dest => dest.aud_fecha, opt => opt.Ignore()); // Opcional: Ignorar si el servicio los asigna
        }
    }
}
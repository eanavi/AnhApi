using AnhApi.Esquemas;
using AnhApi.Modelos.prm;
using AutoMapper;

namespace AnhApi.Mapeos
{
    public class PerfilLocalidad : Profile
    {
        public PerfilLocalidad()
        {
            // Mapeo de DTO de Creación a Modelo de Dominio
            CreateMap<LocalidadCreacion, Localidad>()
                .ForMember(dest => dest.id_localidad, opt => opt.Ignore()) // ID es autoincremental
                .ForMember(dest => dest.id_provincia, opt => opt.MapFrom(src => src.IdProvincia))
                .ForMember(dest => dest.nombre_localidad, opt => opt.MapFrom(src => src.NombreLocalidad))
                // Ignorar campos de auditoría, se manejan en el servicio
                .ForMember(dest => dest.aud_estado, opt => opt.Ignore());

            // Mapeo de Modelo de Dominio a DTO de Listado
            CreateMap<Localidad, LocalidadListado>()
                .ForMember(dest => dest.IdLocalidad, opt => opt.MapFrom(src => src.id_localidad))
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.id_provincia))
                .ForMember(dest => dest.NombreLocalidad, opt => opt.MapFrom(src => src.nombre_localidad));

            // Mapeo de Modelo de Dominio a DTO de Esquema Completo
            CreateMap<Localidad, EsqLocalidad>()
                .ForMember(dest => dest.IdLocalidad, opt => opt.MapFrom(src => src.id_localidad))
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.id_provincia))
                .ForMember(dest => dest.NombreLocalidad, opt => opt.MapFrom(src => src.nombre_localidad))
                // Mapear campos de auditoría
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado));

            // Mapeo de DTO de Esquema Completo a Modelo de Dominio (para Actualización)
            CreateMap<EsqLocalidad, Localidad>()
                .ForMember(dest => dest.id_localidad, opt => opt.MapFrom(src => src.IdLocalidad))
                .ForMember(dest => dest.id_provincia, opt => opt.MapFrom(src => src.IdProvincia))
                .ForMember(dest => dest.nombre_localidad, opt => opt.MapFrom(src => src.NombreLocalidad))
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado));

        }


    }
}
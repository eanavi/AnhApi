using AutoMapper;
using AnhApi.Modelos.prm;
using AnhApi.Esquemas;

namespace AnhApi.Mapeos
{
    public class PerfilMunicipio : Profile
    {
        public PerfilMunicipio()
        {
            // Mapeo de DTO de Creación a Modelo de Dominio
            CreateMap<MunicipioCreacion, Municipio>()
                .ForMember(dest => dest.id_municipio, opt => opt.Ignore())
                .ForMember(dest => dest.id_provincia, opt => opt.MapFrom(src => src.IdProvincia))
                .ForMember(dest => dest.nombre_municipio, opt => opt.MapFrom(src => src.NombreMunicipio))
                .ForMember(dest => dest.seccion, opt => opt.MapFrom(src => src.Seccion))
                .ForMember(dest => dest.aud_estado, opt => opt.Ignore()); // El servicio se encargará de los campos de auditoría

            // Mapeo de Modelo de Dominio a DTO de Listado
            CreateMap<Municipio, MunicipioListado>()
                .ForMember(dest => dest.IdMunicipio, opt => opt.MapFrom(src => src.id_municipio))
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.id_provincia))
                .ForMember(dest => dest.NombreMunicipio, opt => opt.MapFrom(src => src.nombre_municipio))
                .ForMember(dest => dest.Seccion, opt => opt.MapFrom(src => src.seccion));

            // Mapeo de Modelo de Dominio a DTO de Esquema Completo
            CreateMap<Municipio, EsqMunicipio>()
                .ForMember(dest => dest.IdMunicipio, opt => opt.MapFrom(src => src.id_municipio))
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.id_provincia))
                .ForMember(dest => dest.NombreMunicipio, opt => opt.MapFrom(src => src.nombre_municipio))
                .ForMember(dest => dest.Seccion, opt => opt.MapFrom(src => src.seccion))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado));

            // Mapeo de DTO de Esquema Completo a Modelo de Dominio (para Actualización)
            CreateMap<EsqMunicipio, Municipio>()
                .ForMember(dest => dest.id_municipio, opt => opt.MapFrom(src => src.IdMunicipio))
                .ForMember(dest => dest.id_provincia, opt => opt.MapFrom(src => src.IdProvincia))
                .ForMember(dest => dest.nombre_municipio, opt => opt.MapFrom(src => src.NombreMunicipio))
                .ForMember(dest => dest.seccion, opt => opt.MapFrom(src => src.Seccion))
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado));

        }
    }
}

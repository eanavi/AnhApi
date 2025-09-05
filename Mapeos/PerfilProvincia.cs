using AnhApi.Esquemas;
using AnhApi.Modelos.prm;
using AutoMapper;

namespace AnhApi.Mapeos
{
    public class PerfilProvincia : Profile
    {
        public PerfilProvincia()
        {
            CreateMap<ProvinciaCreacion, Provincia>()
                // Ignorar la clave primaria ya que es generada por la BD
                .ForMember(dest => dest.id_provincia, opt => opt.Ignore())
                .ForMember(dest => dest.id_provincia_aux, opt => opt.MapFrom(src => src.IdProvinciaAux))
                .ForMember(dest => dest.id_departamento, opt => opt.MapFrom(src => src.IdDepartamento))
                .ForMember(dest => dest.nombre_provincia, opt => opt.MapFrom(src => src.NombreProvincia))
                // Ignorar campos de auditoría, se manejarán en la capa de servicio
                .ForMember(dest => dest.aud_estado, opt => opt.Ignore());

            CreateMap<Provincia, ProvinciaListado>()
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.id_provincia))
                .ForMember(dest => dest.IdProvinciaAux, opt => opt.MapFrom(src => src.id_provincia_aux))
                .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.id_departamento))
                .ForMember(dest => dest.NombreProvincia, opt => opt.MapFrom(src => src.nombre_provincia));

            CreateMap<Provincia, EsqProvincia>()
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.id_provincia))
                .ForMember(dest => dest.IdProvinciaAux, opt => opt.MapFrom(src => src.id_provincia_aux))
                .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.id_departamento))
                .ForMember(dest => dest.NombreProvincia, opt => opt.MapFrom(src => src.nombre_provincia))
                // Mapear campos de auditoría si se desean exponer en el DTO completo
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado));

            CreateMap<EsqProvincia, Provincia>()
                .ForMember(dest => dest.id_provincia, opt => opt.MapFrom(src => src.IdProvincia))
                .ForMember(dest => dest.id_provincia_aux, opt => opt.MapFrom(src => src.IdProvinciaAux))
                .ForMember(dest => dest.id_departamento, opt => opt.MapFrom(src => src.IdDepartamento))
                .ForMember(dest => dest.nombre_provincia, opt => opt.MapFrom(src => src.NombreProvincia));

            CreateMap<Provincia, ProvConMunicipiosEsq>()
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.id_provincia))
                .ForMember(dest => dest.IdProvinciaAux, opt => opt.MapFrom(src => src.id_provincia_aux))
                .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.id_departamento))
                .ForMember(dest => dest.NombreProvincia, opt => opt.MapFrom(src => src.nombre_provincia))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.Municipios, opt => opt.MapFrom(src => src.Municipios));

            CreateMap<Provincia, ProvConLocalidadesEsq>()
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.id_provincia))
                .ForMember(dest => dest.IdProvinciaAux, opt => opt.MapFrom(src => src.id_provincia_aux))
                .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.id_departamento))
                .ForMember(dest => dest.NombreProvincia, opt => opt.MapFrom(src => src.nombre_provincia))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.Localidades, opt => opt.MapFrom(src => src.Localidades));
        }
    }
}

using AutoMapper;

namespace AnhApi.Mapeos
{
    public class PerfilPerfil : Profile
    {
        public PerfilPerfil() 
        {
            CreateMap<Esquemas.PerfilCreacion, Modelos.Perfil>()
                .ForMember(dest => dest.IdPerfil, opt => opt.Ignore())
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.Id_Unidad, opt => opt.MapFrom(src => src.Id_Unidad));

            CreateMap<Modelos.Perfil, Esquemas.PerfilListado>()
                .ForMember(dest => dest.IdPerfil, opt => opt.MapFrom(src => src.IdPerfil))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.Id_Unidad, opt => opt.MapFrom(src => src.Id_Unidad));

            CreateMap<Modelos.Perfil, Esquemas.EsqPerfil>()
                .ForMember(dest => dest.IdPerfil, opt => opt.MapFrom(src => src.IdPerfil))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.Id_Unidad, opt => opt.MapFrom(src => src.Id_Unidad));

            CreateMap<Esquemas.EsqPerfil, Modelos.Perfil>()
                .ForMember(dest => dest.IdPerfil, opt => opt.MapFrom(src => src.IdPerfil))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.Id_Unidad, opt => opt.MapFrom(src => src.Id_Unidad));
        }
    }
}
using AnhApi.Esquemas;
using AnhApi.Modelos;
using AutoMapper;

namespace AnhApi.Mapeos
{
    public class PerfilCategoriaActividad : Profile
    {
        public PerfilCategoriaActividad()
        {
            // De esquema de creación → entidad
            CreateMap<CategoriaActividadCreacion, CategoriaActividad>();

            // De entidad → esquema de listado
            CreateMap<CategoriaActividad, CategoriaActividadListado>()
                .ForMember(dest => dest.IdCategoriaActividad,
                           opt => opt.MapFrom(src => src.id_categoria_actividad))
                .ForMember(dest => dest.CategoriaPadre,
                           opt => opt.MapFrom(src => src.CategoriaPadre != null ? src.CategoriaPadre.nombre_categoria_actividad : null))
                .ForMember(dest => dest.NombreCategoriaPadre,
                           opt => opt.MapFrom(src => src.nombre_categoria_actividad));

            // De esquema completo → entidad
            CreateMap<EsqCategoriaActividad, CategoriaActividad>()
                .ForMember(dest => dest.id_categoria_actividad,
                           opt => opt.MapFrom(src => src.IdCategoriaActividad ?? 0))
                .ForMember(dest => dest.aud_estado,
                           opt => opt.MapFrom(src => src.AudEstado ?? 0))
                .ForMember(dest => dest.aud_usuario,
                           opt => opt.MapFrom(src => src.AudUsuario))
                .ForMember(dest => dest.aud_fecha,
                           opt => opt.MapFrom(src => src.AudFecha))
                .ForMember(dest => dest.aud_ip,
                           opt => opt.MapFrom(src => src.AudIp));

            // De entidad → esquema completo
            CreateMap<CategoriaActividad, EsqCategoriaActividad>()
                .ForMember(dest => dest.IdCategoriaActividad,
                           opt => opt.MapFrom(src => src.id_categoria_actividad))
                .ForMember(dest => dest.IdPadreCategoriaActividad,
                           opt => opt.MapFrom(src => src.id_padre_categoria_actividad))
                .ForMember(dest => dest.IdOrganigrama,
                           opt => opt.MapFrom(src => src.id_organigrama))
                .ForMember(dest => dest.NombreCategoriaActividad,
                           opt => opt.MapFrom(src => src.nombre_categoria_actividad))
                .ForMember(dest => dest.AudEstado,
                           opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.AudUsuario,
                           opt => opt.MapFrom(src => src.aud_usuario))
                .ForMember(dest => dest.AudFecha,
                           opt => opt.MapFrom(src => src.aud_fecha))
                .ForMember(dest => dest.AudIp,
                           opt => opt.MapFrom(src => src.aud_ip));

        }
    }
}

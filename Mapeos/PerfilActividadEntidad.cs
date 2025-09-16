using AutoMapper;
using AnhApi.Esquemas;
using AnhApi.Modelos;

namespace AnhApi.Mapeos
{
    public class PerfilActividadEntidad : Profile
    {
        public PerfilActividadEntidad()
        {
            CreateMap<Esquemas.ActividadEntidadCreacion, Modelos.ActividadEntidad>()
                // Ignorar la clave primaria ya que es generada por la BD
                .ForMember(dest => dest.id_actividad_entidad, opt => opt.Ignore())
                .ForMember(dest => dest.id_entidad, opt => opt.MapFrom(src => src.Id_Entidad))
                .ForMember(dest => dest.id_actividad, opt => opt.MapFrom(src => src.Id_Actividad))
                // Ignorar campos de auditoría, se manejarán en la capa de servicio
                .ForMember(dest => dest.aud_estado, opt => opt.Ignore());

            CreateMap<Modelos.ActividadEntidad, Esquemas.ActividadEntidadListado>()
                .ForMember(dest => dest.Id_Actividad_Entidad, opt => opt.MapFrom(src => src.id_actividad_entidad))
                .ForMember(dest => dest.Id_Entidad, opt => opt.MapFrom(src => src.id_entidad))
                .AfterMap((src, dest) =>
                {
                    // Mapear el nombre de la entidad si la propiedad de navegación está cargada
                    if (src.Entidad != null)
                    {
                        dest.Entidad = src.Entidad.denominacion;
                    }
                    else
                    {
                        dest.Entidad = null; // O algún valor por defecto
                    }
                    // Mapear el nombre de la actividad si la propiedad de navegación está cargada
                    if (src.Actividad != null)
                    {
                        dest.Actividad = src.Actividad.nombre_actividad;
                    }
                    else
                    {
                        dest.Actividad = null; // O algún valor por defecto
                    }
                });
            CreateMap<Modelos.ActividadEntidad, Esquemas.EsqActividadEntidad>()
                .ForMember(dest => dest.Id_Actividad_Entidad, opt => opt.MapFrom(src => src.id_actividad_entidad))
                .ForMember(dest => dest.Id_Entidad, opt => opt.MapFrom(src => src.id_entidad))
                .ForMember(dest => dest.Id_Actividad, opt => opt.MapFrom(src => src.id_actividad));
                // Mapear campos de auditoría si se desean exponer en el DTO completo
      

            CreateMap<Esquemas.EsqActividadEntidad, Modelos.ActividadEntidad>()
                .ForMember(dest => dest.id_actividad_entidad, opt => opt.MapFrom(src => src.Id_Actividad_Entidad))
                .ForMember(dest => dest.id_entidad, opt => opt.MapFrom(src => src.Id_Entidad))
                .ForMember(dest => dest.id_actividad, opt => opt.MapFrom(src => src.Id_Actividad));
        }
    }
}

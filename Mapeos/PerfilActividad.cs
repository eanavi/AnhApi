using AnhApi.Esquemas;
using AnhApi.Modelos;
using AutoMapper;

namespace AnhApi.Mapeos
{
    public class ActividadProfile : Profile
    {
        public ActividadProfile()
        {
            // Mapeo de ActividadCreacion (DTO) a Actividad (Modelo)
            CreateMap<ActividadCreacion, Actividad>()
                .ForMember(dest => dest.id_actividad, opt => opt.Ignore()) // ID es generado por la BD
                .ForMember(dest => dest.id_categoria_actividad, opt => opt.MapFrom(src => src.IdCategoriaActividad))
                .ForMember(dest => dest.id_operacion, opt => opt.MapFrom(src => src.IdOperacion))
                .ForMember(dest => dest.id_organigrama, opt => opt.MapFrom(src => src.IdOrganigrama))
                .ForMember(dest => dest.nombre_actividad, opt => opt.MapFrom(src => src.NombreActividad))
                .ForMember(dest => dest.tipo_licencia, opt => opt.MapFrom(src => src.TipoLicencia))
                .ForMember(dest => dest.mnemonico, opt => opt.MapFrom(src => src.Mnemonico))
                .ForMember(dest => dest.CategoriaActividad, opt => opt.Ignore())
                .ForMember(dest => dest.Organigrama, opt => opt.Ignore());

            // Mapeo de Actividad (Modelo) a EsqActividad (DTO completo de salida)
            CreateMap<Actividad, EsqActividad>()
                .ForMember(dest => dest.IdActividad, opt => opt.MapFrom(src => src.id_actividad))
                .ForMember(dest => dest.IdCategoriaActividad, opt => opt.MapFrom(src => src.id_categoria_actividad))
                .ForMember(dest => dest.IdOperacion, opt => opt.MapFrom(src => src.id_operacion))
                .ForMember(dest => dest.IdOrganigrama, opt => opt.MapFrom(src => src.id_organigrama))
                .ForMember(dest => dest.NombreActividad, opt => opt.MapFrom(src => src.nombre_actividad))
                .ForMember(dest => dest.TipoLicencia, opt => opt.MapFrom(src => src.tipo_licencia))
                .ForMember(dest => dest.Mnemonico, opt => opt.MapFrom(src => src.mnemonico))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.AudUsuario, opt => opt.MapFrom(src => src.aud_usuario))
                .ForMember(dest => dest.AudFecha, opt => opt.MapFrom(src => src.aud_fecha))
                .ForMember(dest => dest.AudIp, opt => opt.MapFrom(src => src.aud_ip));

            // Mapeo de EsqActividad (DTO) a Actividad (Modelo) para actualizaciones
            CreateMap<EsqActividad, Actividad>()
                .ForMember(dest => dest.id_actividad, opt => opt.MapFrom(src => src.IdActividad))
                .ForMember(dest => dest.id_categoria_actividad, opt => opt.MapFrom(src => src.IdCategoriaActividad))
                .ForMember(dest => dest.id_operacion, opt => opt.MapFrom(src => src.IdOperacion))
                .ForMember(dest => dest.id_organigrama, opt => opt.MapFrom(src => src.IdOrganigrama))
                .ForMember(dest => dest.nombre_actividad, opt => opt.MapFrom(src => src.NombreActividad))
                .ForMember(dest => dest.tipo_licencia, opt => opt.MapFrom(src => src.TipoLicencia))
                .ForMember(dest => dest.mnemonico, opt => opt.MapFrom(src => src.Mnemonico))
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado))
                .ForMember(dest => dest.aud_usuario, opt => opt.MapFrom(src => src.AudUsuario))
                .ForMember(dest => dest.aud_fecha, opt => opt.MapFrom(src => src.AudFecha))
                .ForMember(dest => dest.aud_ip, opt => opt.MapFrom(src => src.AudIp))
                .ForMember(dest => dest.CategoriaActividad, opt => opt.Ignore())
                .ForMember(dest => dest.Organigrama, opt => opt.Ignore());
        }
    }
}
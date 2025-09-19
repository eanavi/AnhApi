using AnhApi.Esquemas;
using AnhApi.Modelos;
using AutoMapper;

namespace AnhApi.Mapeos
{
    public class PerfilOperacion : Profile
    {
        public PerfilOperacion() 
        { 
            CreateMap<OperacionCreacion, Operacion>()
                .ForMember(dest => dest.id_operacion, opt => opt.Ignore())
                .ForMember(dest => dest.nombre_operacion, opt => opt.MapFrom(src => src.NombreOperacion))
                .ForMember(dest => dest.prefijo, opt => opt.MapFrom(src => src.Prefijo))
                .ForMember(dest => dest.prefijo_nombre, opt => opt.MapFrom(src => src.PrefijoNombre))
                .ForMember(dest => dest.desde, opt => opt.MapFrom(src => src.Desde))
                .ForMember(dest => dest.hasta, opt => opt.MapFrom(src => src.Hasta))
                .ForMember(dest => dest.id_anterior, opt => opt.MapFrom(src => src.IdAnterior))
                .ForMember(dest => dest.id_operacion_padre, opt => opt.MapFrom(src => src.IdOperacionPadre))
                .ForMember(dest => dest.id_correlativo_config, opt => opt.MapFrom(src => src.IdCorrelativoConfig))
                .ForMember(dest => dest.nivel, opt => opt.MapFrom(src => src.Nivel))   
                .ForMember(dest => dest.aud_estado, opt => opt.Ignore())
                .ForMember(dest => dest.aud_usuario, opt => opt.Ignore())
                .ForMember(dest => dest.aud_fecha, opt => opt.Ignore())
                .ForMember(dest => dest.aud_ip, opt => opt.Ignore());


            CreateMap<Operacion, EsqOperacion>()
                .ForMember(dest => dest.IdOperacion, opt => opt.MapFrom(src => src.id_operacion))
                .ForMember(dest => dest.NombreOperacion, opt => opt.MapFrom(src => src.nombre_operacion))
                .ForMember(dest => dest.Prefijo, opt => opt.MapFrom(src => src.prefijo))
                .ForMember(dest => dest.PrefijoNombre, opt => opt.MapFrom(src => src.prefijo_nombre))
                .ForMember(dest => dest.Desde, opt => opt.MapFrom(src => src.desde))
                .ForMember(dest => dest.Hasta, opt => opt.MapFrom(src => src.hasta))
                .ForMember(dest => dest.IdAnterior, opt => opt.MapFrom(src => src.id_anterior))
                .ForMember(dest => dest.IdOperacionPadre, opt => opt.MapFrom(src => src.id_operacion_padre))
                .ForMember(dest => dest.IdCorrelativoConfig, opt => opt.MapFrom(src => src.id_correlativo_config))
                .ForMember(dest => dest.Nivel, opt => opt.MapFrom(src => src.nivel))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.AudUsuario, opt => opt.MapFrom(src => src.aud_usuario))
                .ForMember(dest => dest.AudFecha, opt => opt.MapFrom(src => src.aud_fecha))
                .ForMember(dest => dest.AudIp, opt => opt.MapFrom(src => src.aud_ip));

            CreateMap<EsqOperacion, Operacion>()
                .ForMember(dest => dest.id_operacion, opt => opt.MapFrom(src => src.IdOperacion))
                .ForMember(dest => dest.nombre_operacion, opt => opt.MapFrom(src => src.NombreOperacion))
                .ForMember(dest => dest.prefijo, opt => opt.MapFrom(src => src.Prefijo))
                .ForMember(dest => dest.prefijo_nombre, opt => opt.MapFrom(src => src.PrefijoNombre))
                .ForMember(dest => dest.desde, opt => opt.MapFrom(src => src.Desde))
                .ForMember(dest => dest.hasta, opt => opt.MapFrom(src => src.Hasta))
                .ForMember(dest => dest.id_anterior, opt => opt.MapFrom(src => src.IdAnterior))
                .ForMember(dest => dest.id_operacion_padre, opt => opt.MapFrom(src => src.IdOperacionPadre))
                .ForMember(dest => dest.id_correlativo_config, opt => opt.MapFrom(src => src.IdCorrelativoConfig))
                .ForMember(dest => dest.nivel, opt => opt.MapFrom(src => src.Nivel))
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado))
                .ForMember(dest => dest.aud_usuario, opt => opt.MapFrom(src => src.AudUsuario))
                .ForMember(dest => dest.aud_fecha, opt => opt.MapFrom(src => src.AudFecha))
                .ForMember(dest => dest.aud_ip, opt => opt.MapFrom(src => src.AudIp));
        }




    }
}

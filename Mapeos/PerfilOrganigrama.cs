using AnhApi.Esquemas;
using AnhApi.Modelos;
using AutoMapper;

namespace AnhApi.Mapeos
{
    public class PerfilOrganigrama : Profile
    {
        public PerfilOrganigrama()
        {
            CreateMap<OrganigramaCreacion, Organigrama>()
                .ForMember(dest => dest.id_organigrama, opt => opt.Ignore())
                .ForMember(dest => dest.id_organigrama_padre, opt => opt.MapFrom(src => src.IdOrganigramaPadre))
                .ForMember(dest => dest.id_departamento, opt => opt.MapFrom(src => src.IdDepartamento))
                .ForMember(dest => dest.id_sirh, opt => opt.MapFrom(src => src.IdSirh))
                .ForMember(dest => dest.nombre_organigrama, opt => opt.MapFrom(src => src.NombreOrganigrama))
                .ForMember(dest => dest.sigla, opt => opt.MapFrom(src => src.Sigla))
                .ForMember(dest => dest.nivel, opt => opt.MapFrom(src => src.Nivel))
                .ForMember(dest => dest.tipo_organigrama, opt => opt.MapFrom(src => src.TipoOrganigrama))
                .ForMember(dest => dest.clase_organigrama, opt => opt.MapFrom(src => src.ClaseOrganigrama))
                .ForMember(dest => dest.OrganigramaPadre, opt => opt.Ignore());

            CreateMap<Organigrama, EsqOrganigrama>()
                .ForMember(dest => dest.IdOrganigrama, opt => opt.MapFrom(src => src.id_organigrama))
                .ForMember(dest => dest.IdOrganigramaPadre, opt => opt.MapFrom(src => src.id_organigrama_padre))
                .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.id_departamento))
                .ForMember(dest => dest.IdSirh, opt => opt.MapFrom(src => src.id_sirh))
                .ForMember(dest => dest.NombreOrganigrama, opt => opt.MapFrom(src => src.nombre_organigrama))
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.sigla))
                .ForMember(dest => dest.Nivel, opt => opt.MapFrom(src => src.nivel))
                .ForMember(dest => dest.TipoOrganigrama, opt => opt.MapFrom(src => src.tipo_organigrama))
                .ForMember(dest => dest.ClaseOrganigrama, opt => opt.MapFrom(src => src.clase_organigrama))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.AudUsuario, opt => opt.MapFrom(src => src.aud_usuario))
                .ForMember(dest => dest.AudFecha, opt => opt.MapFrom(src => src.aud_fecha))
                .ForMember(dest => dest.AudIp, opt => opt.MapFrom(src => src.aud_ip));

            CreateMap<EsqOrganigrama, Organigrama>()
                .ForMember(dest => dest.id_organigrama, opt => opt.MapFrom(src => src.IdOrganigrama))
                .ForMember(dest => dest.id_organigrama_padre, opt => opt.MapFrom(src => src.IdOrganigramaPadre))
                .ForMember(dest => dest.id_departamento, opt => opt.MapFrom(src => src.IdDepartamento))
                .ForMember(dest => dest.id_sirh, opt => opt.MapFrom(src => src.IdSirh))
                .ForMember(dest => dest.nombre_organigrama, opt => opt.MapFrom(src => src.NombreOrganigrama))
                .ForMember(dest => dest.sigla, opt => opt.MapFrom(src => src.Sigla))
                .ForMember(dest => dest.nivel, opt => opt.MapFrom(src => src.Nivel))
                .ForMember(dest => dest.tipo_organigrama, opt => opt.MapFrom(src => src.TipoOrganigrama))
                .ForMember(dest => dest.clase_organigrama, opt => opt.MapFrom(src => src.ClaseOrganigrama))
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado))
                .ForMember(dest => dest.aud_usuario, opt => opt.MapFrom(src => src.AudUsuario))
                .ForMember(dest => dest.aud_fecha, opt => opt.MapFrom(src => src.AudFecha))
                .ForMember(dest => dest.aud_ip, opt => opt.MapFrom(src => src.AudIp))
                .ForMember(dest => dest.OrganigramaPadre, opt => opt.Ignore());
        }
    }
}
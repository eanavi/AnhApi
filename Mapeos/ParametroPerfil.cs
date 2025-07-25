using AutoMapper;
using AnhApi.Modelos;
using AnhApi.Esquemas;


namespace AnhApi.Mapeos
{
    public class ParametroPerfil : Profile
    {
        public ParametroPerfil()
        {
            // Mapeo desde el DTO de creación (ParametroCreacion) al Modelo (Modelos.Parametro)
            CreateMap<ParametroCreacion, Parametro>()
                .ForMember(dest => dest.id_parametro, opt => opt.Ignore())
                .ForMember(dest => dest.codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.sigla, opt => opt.MapFrom(src => src.Sigla))
                .ForMember(dest => dest.grupo, opt => opt.MapFrom(src => src.Grupo));

            // --- Mapeo para el LISTADO / OBTENER POR ID (GET) ---
            // Desde Modelos.Parametro (entidad de BD) a Esquemas.ParametroListado (DTO de salida)
            // Este DTO contiene IdParametro, Codigo, Descripcion, Sigla, Grupo
            CreateMap<Parametro, ParametroListado>()
                .ForMember(dest => dest.IdParametro, opt => opt.MapFrom(src => src.id_parametro))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.codigo))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.descripcion))
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.sigla))
                .ForMember(dest => dest.Grupo, opt => opt.MapFrom(src => src.grupo));

            CreateMap<Parametro, ParametroCmbLit>()
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.sigla))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.descripcion));

            CreateMap<Parametro, ParametroCmb>()
                .ForMember(dest => dest.Codigo, op => op.MapFrom(src => src.codigo))
                .ForMember(dest => dest.Descripcion, op => op.MapFrom(src => src.descripcion));

            // --- Mapeo para el Esquema General / Completo (ParametroEsq) ---
            // Desde Modelos.Parametro (entidad de BD) a Esquemas.ParametroEsq (DTO de salida)
            // ParametroEsq hereda de ParametroListado, así que los campos de ParametroListado se mapean automáticamente
            // debido a la herencia de AutoMapper. Solo necesitamos mapear lo adicional.
            CreateMap<Parametro, ParametroEsq>()
                // Map all properties explicitly, including those inherited from ParametroListado
                .ForMember(dest => dest.IdParametro, opt => opt.MapFrom(src => src.id_parametro))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.codigo))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.descripcion))
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.sigla))
                .ForMember(dest => dest.Grupo, opt => opt.MapFrom(src => src.grupo))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado)); // And its own property

            // --- Mapeo para la ACTUALIZACIÓN (PUT/PATCH) ---
            // Desde Esquemas.ParametroEsq (DTO de entrada) a Modelos.Parametro (entidad de BD)
            // ParametroEsq ahora contiene todos los campos necesarios para una actualización completa.
            CreateMap<ParametroEsq, Parametro>()
                .ForMember(dest => dest.id_parametro, opt => opt.MapFrom(src => src.IdParametro))
                .ForMember(dest => dest.codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.sigla, opt => opt.MapFrom(src => src.Sigla))
                .ForMember(dest => dest.grupo, opt => opt.MapFrom(src => src.Grupo))
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado));
        }
    }
}

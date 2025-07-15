using AutoMapper;

namespace AnhApi.Mapeos
{
    public class UsuarioPerfil : Profile
    {
        public UsuarioPerfil()
        {
            // --- Mapeo para la creacion (POST) ---
            // De Esquemas.UsuarioCreacion entrada DTO a Modelos.Usuario (DB entity)
            CreateMap<Esquemas.UsuarioCreacion, Modelos.Usuario>()
                .ForMember(dest => dest.id_usuario, opt => opt.Ignore()) // ID is DB generated
                .ForMember(dest => dest.id_persona, opt => opt.MapFrom(src => src.IdPersona))
                .ForMember(dest => dest.login, opt => opt.MapFrom(src => src.Login))
                .ForMember(dest => dest.clave, opt => opt.MapFrom(src => src.Clave));
            // Asumiendo que los campos de auditoria los asume el controlador 

            // --- Mapeo para el Listado (GET /api/usuarios) ---
            // de Modelos.Usuario (DB entity) a Esquemas.UsuarioListado (Salida a DTO)
            CreateMap<Modelos.Usuario, Esquemas.UsuarioListado>()
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.id_usuario))
                .ForMember(dest => dest.IdPersona, opt => opt.MapFrom(src => src.id_persona))
                .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.login));


            // --- Mapeo for Equema completo (GET /api/usuarios/{id}, PUT/PATCH) ---
            // De Modelos.Usuario (DB entity) a Esquemas.UsuarioEsq (Salida completa DTO)
            // UsuarioEsq hereda de UsuarioListado, los cuales estan incluidos en la base.
            CreateMap<Modelos.Usuario, Esquemas.UsuarioEsq>()
                .IncludeBase<Modelos.Usuario, Esquemas.UsuarioListado>() // AutoMapper will apply the base mapping
                .ForMember(dest => dest.Clave, opt => opt.MapFrom(src => src.clave));
 
            // --- Mapeo para actualizacion UPDATING (PUT/PATCH) ---
            // de Esquemas.UsuarioEsq (full input DTO) a Modelos.Usuario (DB entity)
            CreateMap<Esquemas.UsuarioEsq, Modelos.Usuario>()
                .ForMember(dest => dest.id_usuario, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.id_persona, opt => opt.MapFrom(src => src.IdPersona))
                .ForMember(dest => dest.login, opt => opt.MapFrom(src => src.Login))
                .ForMember(dest => dest.clave, opt => opt.MapFrom(src => src.Clave));






        }
    }
}

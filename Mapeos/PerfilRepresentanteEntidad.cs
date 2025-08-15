using AutoMapper;
using AnhApi.Modelos;
using AnhApi.Esquemas;


namespace AnhApi.Mapeos
{
    public class PerfilRepresentanteEntidad : Profile
    {
        public PerfilRepresentanteEntidad()
        {

            CreateMap<RepresentanteEntidad, Persona>()
                .ForMember(dest => dest.id_persona, opt => opt.MapFrom(src => src.Persona.id_persona))
                .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.Persona.nombre))
                .ForMember(dest => dest.primer_apellido, opt => opt.MapFrom(src => src.Persona.primer_apellido))
                .ForMember(dest => dest.segundo_apellido, opt => opt.MapFrom(src => src.Persona.segundo_apellido));
               
        }
    }
}

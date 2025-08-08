using AutoMapper;
using AnhApi.Modelos;
using AnhApi.Esquemas;
using System.Text.Json;

namespace AnhApi.Mapeos
{
    public class PersonaProfile : Profile
    {
        public PersonaProfile()
        {
            // Mapeo desde el DTO de creación (PersonaCreacion) al Modelo (Modelos.Persona)
            CreateMap<PersonaCreacion, Persona>()
                .ForMember(dest => dest.id_persona, opt => opt.Ignore()) // ID es generado por la BD
                .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.primer_apellido, opt => opt.MapFrom(src => src.PrimerApellido))
                .ForMember(dest => dest.segundo_apellido, opt => opt.MapFrom(src => src.SegundoApellido))
                .ForMember(dest => dest.fecha_nacimiento, opt => opt.MapFrom(src => src.FechaNacimiento))
                .ForMember(dest => dest.numero_identificacion, opt => opt.MapFrom(src => src.NumeroIdentificacion))
                .ForMember(dest => dest.complemento, opt => opt.MapFrom(src => src.Complemento))
                .ForMember(dest => dest.genero, opt => opt.MapFrom(src => src.Genero))
                .ForMember(dest => dest.direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.telefono, opt => opt.MapFrom(src => src.Telefono))
                .ForMember(dest => dest.correo, opt => opt.MapFrom(src => src.Correo));
            // Los campos de auditoría (aud_estado, etc.) no están en PersonaCreacion,
            // se asume que los asignas en el servicio (ej. 'aud_estado = 0', 'aud_usuario = "sistema"')
            // Mapeo para actualizar desde el Esquema (EsqPersona) al Modelo (Modelos.Persona)


            // --- Mapeo para el LISTADO (GET /api/personas) ---
            // Desde Modelos.Persona (entidad de BD) a Esquemas.PersonaListado (DTO de salida más ligero)
            CreateMap<Persona, PersonaListado>()
                .ForMember(dest => dest.IdPersona, opt => opt.MapFrom(src => src.id_persona))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.nombre))
                .ForMember(dest => dest.PrimerApellido, opt => opt.MapFrom(src => src.primer_apellido))
                .ForMember(dest => dest.SegundoApellido, opt => opt.MapFrom(src => src.segundo_apellido))
                .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => src.fecha_nacimiento))
                .ForMember(dest => dest.NumeroIdentificacion, opt => opt.MapFrom(src => src.numero_identificacion))
                .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.complemento))
                .ForMember(dest => dest.Genero, opt => opt.MapFrom(src => src.genero));
            // Aquí NO se mapean Direccion, Telefono ni Correo, lo cual es correcto para este DTO.


            // --- Mapeo para el Esquema Completo / Detallado (GET /api/personas/{id}, PUT/PATCH) ---
            // Desde Modelos.Persona (entidad de BD) a Esquemas.Persona (DTO de salida completo)
            CreateMap<Persona, EsqPersona>()
                .ForMember(dest => dest.IdPersona, opt => opt.MapFrom(src => src.id_persona))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.nombre))
                .ForMember(dest => dest.PrimerApellido, opt => opt.MapFrom(src => src.primer_apellido))
                .ForMember(dest => dest.SegundoApellido, opt => opt.MapFrom(src => src.segundo_apellido))
                .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => src.fecha_nacimiento))
                .ForMember(dest => dest.NumeroIdentificacion, opt => opt.MapFrom(src => src.numero_identificacion))
                .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.complemento))
                .ForMember(dest => dest.Genero, opt => opt.MapFrom(src => src.genero))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.direccion))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.telefono))
                .ForMember(dest => dest.Correo, opt => opt.MapFrom(src => src.correo))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.AudUsuario, opt => opt.MapFrom(src => src.aud_usuario))
                .ForMember(dest => dest.AudFecha, opt => opt.MapFrom(src => src.aud_fecha))
                .ForMember(dest => dest.AudIp, opt => opt.MapFrom(src => src.aud_ip));

            // --- Mapeo para la ACTUALIZACIÓN (PUT/PATCH) ---
            // Desde Esquemas.Persona (DTO de entrada completo) a Modelos.Persona (entidad de BD)
            // Este DTO contiene todos los campos necesarios para una actualización.
            CreateMap<EsqPersona, Modelos.Persona>()
                .ForMember(dest => dest.id_persona, opt => opt.MapFrom(src => src.IdPersona)) // El ID debe mapearse para la actualización
                .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.primer_apellido, opt => opt.MapFrom(src => src.PrimerApellido))
                .ForMember(dest => dest.segundo_apellido, opt => opt.MapFrom(src => src.SegundoApellido))
                .ForMember(dest => dest.fecha_nacimiento, opt => opt.MapFrom(src => src.FechaNacimiento))
                .ForMember(dest => dest.numero_identificacion, opt => opt.MapFrom(src => src.NumeroIdentificacion))
                .ForMember(dest => dest.complemento, opt => opt.MapFrom(src => src.Complemento))
                .ForMember(dest => dest.genero, opt => opt.MapFrom(src => src.Genero))
                .ForMember(dest => dest.direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.telefono, opt => opt.MapFrom(src => src.Telefono))
                .ForMember(dest => dest.correo, opt => opt.MapFrom(src => src.Correo))
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado))
                .ForMember(dest => dest.aud_usuario, opt => opt.MapFrom(src => src.AudUsuario))
                .ForMember(dest => dest.aud_fecha, opt => opt.MapFrom(src => src.AudFecha))
                .ForMember(dest => dest.aud_ip, opt => opt.MapFrom(src => src.AudIp));
        }
    }
}
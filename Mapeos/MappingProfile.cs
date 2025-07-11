using AutoMapper;
using AnhApi.Modelos;
using EsqPersona = AnhApi.Esquemas.Persona;
using System.Text.Json;


namespace AnhApi.Mapeos
{
    public class PersonaProfile : Profile
    {
        public PersonaProfile()
        {
            CreateMap<Persona, EsqPersona>()
                .ForMember(dest => dest.IdPersona, opt => opt.MapFrom(src => src.id_persona))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.nombre))
                .ForMember(dest => dest.PrimerApellido, opt => opt.MapFrom(src => src.primer_apellido))
                .ForMember(dest => dest.SegundoApellido, opt => opt.MapFrom(src => src.segundo_apellido))
                .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => src.fecha_nacimiento))
                .ForMember(dest => dest.NumeroIdentificacion, opt => opt.MapFrom(src => src.numero_identificacion))
                .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.complemento))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.direccion != null ? src.direccion.RootElement.ToString() : null))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.telefono != null ? src.telefono.RootElement.ToString() : null))
                .ForMember(dest => dest.Correo, opt => opt.MapFrom(src => src.correo != null ? src.correo.RootElement.ToString() : null)).ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado));

            CreateMap<EsqPersona, Persona>()
                .ForMember(dest => dest.id_persona, opt => opt.MapFrom(src => src.IdPersona))
                .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.primer_apellido, opt => opt.MapFrom(src => src.PrimerApellido))
                .ForMember(dest => dest.segundo_apellido, opt => opt.MapFrom(src => src.SegundoApellido))
                .ForMember(dest => dest.fecha_nacimiento, opt => opt.MapFrom(src => src.FechaNacimiento))
                .ForMember(dest => dest.numero_identificacion, opt => opt.MapFrom(src => src.NumeroIdentificacion))
                .ForMember(dest => dest.complemento, opt => opt.MapFrom(src => src.Complemento))
                .ForMember(dest => dest.direccion, opt => opt.MapFrom((src, dest, _, context) =>
                {
                    if (src.Direccion == null)
                    {
                        return null;
                    }

                    string? direccionString = src.Direccion as string;
                    if (direccionString != null)
                    {
                        try
                        {
                            return JsonDocument.Parse(direccionString);
                        }
                        catch (JsonException ex)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        try
                        {
                            string serialized = JsonSerializer.Serialize(src.Direccion);
                            return JsonDocument.Parse(serialized);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }))
                .ForMember(dest => dest.telefono, opt => opt.MapFrom((src, dest, _, context) =>
                {
                    if (src.Telefono == null)
                    {
                        return null;
                    }
                    string? telefonoString = src.Telefono as string;
                    if (telefonoString != null)
                    {
                        try
                        {
                            return JsonDocument.Parse(telefonoString);
                        }
                        catch (JsonException ex)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        try
                        {
                            string serialized = JsonSerializer.Serialize(src.Telefono);
                            return JsonDocument.Parse(serialized);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }))
                .ForMember(dest => dest.correo, opt => opt.MapFrom((src, dest, _, context) =>
                {
                    if (src.Correo == null)
                    {
                        return null;
                    }
                    string? correoString = src.Correo as string;
                    if (correoString != null)
                    {
                        try
                        {
                            return JsonDocument.Parse(correoString);
                        }
                        catch (JsonException ex)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        try
                        {
                            string serialized = JsonSerializer.Serialize(src.Correo);
                            return JsonDocument.Parse(serialized);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }))
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado));
        }

    }
}
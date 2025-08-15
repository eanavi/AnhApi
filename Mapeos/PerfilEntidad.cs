using AnhApi.Esquemas;
using AnhApi.Modelos;
using AutoMapper;
using System.Text.Json;

namespace AnhApi.Mapeos
{
    public class EntidadProfile : Profile
    {
        public EntidadProfile()
        {
            // Mapeo de EntidadCreacion (DTO) a Entidad (Modelo)
            CreateMap<EntidadCreacion, Entidad>()
                .ForMember(dest => dest.id_entidad, opt => opt.Ignore()) // ID es generado por la BD
                .ForMember(dest => dest.id_entidad_padre, opt => opt.MapFrom(src => src.IdEntidadPadre))
                .ForMember(dest => dest.id_tipo_entidad, opt => opt.MapFrom(src => src.IdTipoEntidad))
                .ForMember(dest => dest.id_tipo_sociedad, opt => opt.MapFrom(src => src.IdTipoSociedad))
                .ForMember(dest => dest.id_ambito_operacion, opt => opt.MapFrom(src => src.IdAmbitoOperacion))
                .ForMember(dest => dest.id_localidad, opt => opt.MapFrom(src => src.IdLocalidad))
                .ForMember(dest => dest.id_municipio, opt => opt.MapFrom(src => src.IdMunicipio))
                .ForMember(dest => dest.id_estado_operacion, opt => opt.MapFrom(src => src.IdEstadoOperacion))
                .ForMember(dest => dest.id_estado_empadronamiento, opt => opt.MapFrom(src => src.IdEstadoEmpadronamiento))
                .ForMember(dest => dest.denominacion, opt => opt.MapFrom(src => src.Denominacion))
                .ForMember(dest => dest.sigla, opt => opt.MapFrom(src => src.Sigla))
                .ForMember(dest => dest.identificacion, opt => opt.MapFrom(src => src.Identificacion))
                .ForMember(dest => dest.tipo_identificacion, opt => opt.MapFrom(src => src.TipoIdentificacion))
                .ForMember(dest => dest.fecha_registro, opt => opt.Ignore()) // Se asigna en el servicio o la BD
                .ForMember(dest => dest.direccion, opt => opt.Ignore())
                .ForMember(dest => dest.telefono, opt => opt.Ignore())
                .ForMember(dest => dest.correo, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    // Lógica para mapear los campos JSONB
                    if (src.Direccion != null)
                    {
                        dest.direccion = JsonDocument.Parse(JsonSerializer.Serialize(src.Direccion));
                    }
                    if (src.Telefono != null)
                    {
                        dest.telefono = JsonDocument.Parse(JsonSerializer.Serialize(src.Telefono));
                    }
                    if (src.Correo != null)
                    {
                        dest.correo = JsonDocument.Parse(JsonSerializer.Serialize(src.Correo));
                    }
                });


            // Mapeo de Entidad (Modelo) a EntidadListado (DTO de salida)
            /*
            CreateMap<Entidad, EntidadListado>()
                .ForMember(dest => dest.IdEntidad, opt => opt.MapFrom(src => src.id_entidad))
                .ForMember(dest => dest.Tipo, opt => opt.Ignore())
                .ForMember(dest => dest.Sociedad, opt => opt.Ignore())
                .ForMember(dest => dest.Area, opt => opt.Ignore())
                .ForMember(dest => dest.Localidad, opt => opt.Ignore())
                .ForMember(dest => dest.Municipio, opt => opt.Ignore())
                .ForMember(dest => dest.Provincia, opt => opt.Ignore())
                .ForMember(dest => dest.Departamento, opt => opt.Ignore())
                .ForMember(dest => dest.Denominacion, opt => opt.MapFrom(src => src.denominacion))
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.sigla))
                .ForMember(dest => dest.Identificacion, opt => opt.MapFrom(src => src.identificacion))
                .ForMember(dest => dest.TipoIdentificacion, opt => opt.MapFrom(src => src.tipo_identificacion))
                .ForMember(dest => dest.EstadoOperacion, opt => opt.MapFrom(src => src.id_estado_operacion)); // Se asume que esto se llenará con un valor de otro servicio
            */
            // Mapeo de Entidad (Modelo) a EsqEntidad (DTO completo de salida)
            CreateMap<Entidad, EsqEntidad>()
                .ForMember(dest => dest.IdEntidad, opt => opt.MapFrom(src => src.id_entidad))
                .ForMember(dest => dest.IdEntidadPadre, opt => opt.MapFrom(src => src.id_entidad_padre))
                .ForMember(dest => dest.IdTipoEntidad, opt => opt.MapFrom(src => src.id_tipo_entidad))
                .ForMember(dest => dest.IdTipoSociedad, opt => opt.MapFrom(src => src.id_tipo_sociedad))
                .ForMember(dest => dest.IdAmbitoOperacion, opt => opt.MapFrom(src => src.id_ambito_operacion))
                .ForMember(dest => dest.IdLocalidad, opt => opt.MapFrom(src => src.id_localidad))
                .ForMember(dest => dest.IdMunicipio, opt => opt.MapFrom(src => src.id_municipio))
                .ForMember(dest => dest.IdEstadoOperacion, opt => opt.MapFrom(src => src.id_estado_operacion))
                .ForMember(dest => dest.IdEstadoEmpadronamiento, opt => opt.MapFrom(src => src.id_estado_empadronamiento))
                .ForMember(dest => dest.Denominacion, opt => opt.MapFrom(src => src.denominacion))
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.sigla))
                .ForMember(dest => dest.Identificacion, opt => opt.MapFrom(src => src.identificacion))
                .ForMember(dest => dest.TipoIdentificacion, opt => opt.MapFrom(src => src.tipo_identificacion))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.direccion))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.telefono))
                .ForMember(dest => dest.Correo, opt => opt.MapFrom(src => src.correo))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.AudUsuario, opt => opt.MapFrom(src => src.aud_usuario))
                .ForMember(dest => dest.AudFecha, opt => opt.MapFrom(src => src.aud_fecha))
                .ForMember(dest => dest.AudIp, opt => opt.MapFrom(src => src.aud_ip));


            // Mapeo de Entidad a EntidadConRep
            CreateMap<Entidad, EntidadConRep>()
                .ForMember(dest => dest.IdEntidad, opt => opt.MapFrom(src => src.id_entidad))
                .ForMember(dest => dest.IdEntidadPadre, opt => opt.MapFrom(src => src.id_entidad_padre))
                .ForMember(dest => dest.IdTipoEntidad, opt => opt.MapFrom(src => src.id_tipo_entidad))
                .ForMember(dest => dest.IdTipoSociedad, opt => opt.MapFrom(src => src.id_tipo_sociedad))
                .ForMember(dest => dest.IdAmbitoOperacion, opt => opt.MapFrom(src => src.id_ambito_operacion))
                .ForMember(dest => dest.IdLocalidad, opt => opt.MapFrom(src => src.id_localidad))
                .ForMember(dest => dest.IdMunicipio, opt => opt.MapFrom(src => src.id_municipio))
                .ForMember(dest => dest.IdEstadoOperacion, opt => opt.MapFrom(src => src.id_estado_operacion))
                .ForMember(dest => dest.IdEstadoEmpadronamiento, opt => opt.MapFrom(src => src.id_estado_empadronamiento))
                .ForMember(dest => dest.Denominacion, opt => opt.MapFrom(src => src.denominacion))
                .ForMember(dest => dest.Sigla, opt => opt.MapFrom(src => src.sigla))
                .ForMember(dest => dest.Identificacion, opt => opt.MapFrom(src => src.identificacion))
                .ForMember(dest => dest.TipoIdentificacion, opt => opt.MapFrom(src => src.tipo_identificacion))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.direccion))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.telefono))
                .ForMember(dest => dest.Correo, opt => opt.MapFrom(src => src.correo))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.AudUsuario, opt => opt.MapFrom(src => src.aud_usuario))
                .ForMember(dest => dest.AudFecha, opt => opt.MapFrom(src => src.aud_fecha))
                .ForMember(dest => dest.AudIp, opt => opt.MapFrom(src => src.aud_ip))
                .ForMember(dest => dest.Representantes, opt => opt.MapFrom(src => src.Representantes));


            // Mapeo de EsqEntidad (DTO) a Entidad (Modelo) para actualizaciones
            CreateMap<EsqEntidad, Entidad>()
                .ForMember(dest => dest.id_entidad, opt => opt.MapFrom(src => src.IdEntidad))
                .ForMember(dest => dest.id_entidad_padre, opt => opt.MapFrom(src => src.IdEntidadPadre))
                .ForMember(dest => dest.id_tipo_entidad, opt => opt.MapFrom(src => src.IdTipoEntidad))
                .ForMember(dest => dest.id_tipo_sociedad, opt => opt.MapFrom(src => src.IdTipoSociedad))
                .ForMember(dest => dest.id_ambito_operacion, opt => opt.MapFrom(src => src.IdAmbitoOperacion))
                .ForMember(dest => dest.id_localidad, opt => opt.MapFrom(src => src.IdLocalidad))
                .ForMember(dest => dest.id_municipio, opt => opt.MapFrom(src => src.IdMunicipio))
                .ForMember(dest => dest.id_estado_operacion, opt => opt.MapFrom(src => src.IdEstadoOperacion))
                .ForMember(dest => dest.id_estado_empadronamiento, opt => opt.MapFrom(src => src.IdEstadoEmpadronamiento))
                .ForMember(dest => dest.denominacion, opt => opt.MapFrom(src => src.Denominacion))
                .ForMember(dest => dest.sigla, opt => opt.MapFrom(src => src.Sigla))
                .ForMember(dest => dest.identificacion, opt => opt.MapFrom(src => src.Identificacion))
                .ForMember(dest => dest.tipo_identificacion, opt => opt.MapFrom(src => src.TipoIdentificacion))
                .ForMember(dest => dest.fecha_registro, opt => opt.Ignore()) // No se actualiza la fecha de registro
                .ForMember(dest => dest.direccion, opt => opt.Ignore())
                .ForMember(dest => dest.telefono, opt => opt.Ignore())
                .ForMember(dest => dest.correo, opt => opt.Ignore())
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado))
                .ForMember(dest => dest.aud_usuario, opt => opt.MapFrom(src => src.AudUsuario))
                .ForMember(dest => dest.aud_fecha, opt => opt.MapFrom(src => src.AudFecha))
                .ForMember(dest => dest.aud_ip, opt => opt.MapFrom(src => src.AudIp))
                .AfterMap((src, dest) =>
                {
                    // Lógica para mapear los campos JSONB en la actualización
                    if (src.Direccion != null)
                    {
                        dest.direccion = JsonDocument.Parse(JsonSerializer.Serialize(src.Direccion));
                    }
                    if (src.Telefono != null)
                    {
                        dest.telefono = JsonDocument.Parse(JsonSerializer.Serialize(src.Telefono));
                    }
                    if (src.Correo != null)
                    {
                        dest.correo = JsonDocument.Parse(JsonSerializer.Serialize(src.Correo));
                    }
                });
        }
    }
}
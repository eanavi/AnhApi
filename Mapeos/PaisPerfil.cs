using AutoMapper;
using AnhApi.Modelos.prm; // Para la clase Pais (modelo de Entity Framework Core)
using AnhApi.Esquemas; // Para las clases PaisCreacion, PaisListado, PaisEsq

namespace AnhApi.Mapeos
{
    public class PaisPerfil: Profile
    {
        public PaisPerfil()
        {
            // --- Mapeo para la CREACIÓN (POST /api/paises) ---
            // Desde el DTO de creación (PaisCreacion) al Modelo (Modelos.Pais)
            CreateMap<PaisCreacion, Pais>()
                .ForMember(dest => dest.id_pais, opt => opt.Ignore()) // ID es autoincremental en la BD (int4) o asignado manualmente si no es IDENTITY
                .ForMember(dest => dest.nombre_pais, opt => opt.MapFrom(src => src.NombrePais))
                .ForMember(dest => dest.abreviacion2, opt => opt.MapFrom(src => src.Abreviacion2))
                .ForMember(dest => dest.abreviacion3, opt => opt.MapFrom(src => src.Abreviacion3))
                .ForMember(dest => dest.codigo_internacional, opt => opt.MapFrom(src => src.CodigoInternacional));
            // Los campos de auditoría (aud_estado, etc.) no están en PaisCreacion;
            // se asume que los asignarás en el servicio (ej. 'aud_estado = 0', 'aud_usuario = "sistema"')
            // antes de guardar la entidad en la base de datos.


            // --- Mapeo para el LISTADO (GET /api/paises) ---
            // Desde Modelos.Pais (entidad de BD) a Esquemas.PaisListado (DTO de salida más ligero)
            CreateMap<Pais, PaisListado>()
                .ForMember(dest => dest.IdPais, opt => opt.MapFrom(src => src.id_pais))
                .ForMember(dest => dest.NombrePais, opt => opt.MapFrom(src => src.nombre_pais))
                .ForMember(dest => dest.Abreviacion2, opt => opt.MapFrom(src => src.abreviacion2))
                .ForMember(dest => dest.Abreviacion3, opt => opt.MapFrom(src => src.abreviacion3))
                .ForMember(dest => dest.CodigoInternacional, opt => opt.MapFrom(src => src.codigo_internacional));


            // --- Mapeo para el Esquema Completo / Detallado (GET /api/paises/{id}, PUT/PATCH) ---
            // Desde Modelos.Pais (entidad de BD) a Esquemas.PaisEsq (DTO de salida completo)
            CreateMap<Pais, PaisEsq>()
                .ForMember(dest => dest.IdPais, opt => opt.MapFrom(src => src.id_pais))
                .ForMember(dest => dest.NombrePais, opt => opt.MapFrom(src => src.nombre_pais))
                .ForMember(dest => dest.Abreviacion2, opt => opt.MapFrom(src => src.abreviacion2))
                .ForMember(dest => dest.Abreviacion3, opt => opt.MapFrom(src => src.abreviacion3))
                .ForMember(dest => dest.CodigoInternacional, opt => opt.MapFrom(src => src.codigo_internacional))

                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado));


            // --- Mapeo para la ACTUALIZACIÓN (PUT/PATCH) ---
            // Desde Esquemas.PaisEsq (DTO de entrada completo) a Modelos.Pais (entidad de BD)
            // Este DTO contiene todos los campos necesarios para una actualización.
            CreateMap<PaisEsq, Pais>()
                .ForMember(dest => dest.id_pais, opt => opt.MapFrom(src => src.IdPais)) // El ID debe mapearse para la actualización
                .ForMember(dest => dest.nombre_pais, opt => opt.MapFrom(src => src.NombrePais))
                .ForMember(dest => dest.abreviacion2, opt => opt.MapFrom(src => src.Abreviacion2))
                .ForMember(dest => dest.abreviacion3, opt => opt.MapFrom(src => src.Abreviacion3))
                .ForMember(dest => dest.codigo_internacional, opt => opt.MapFrom(src => src.CodigoInternacional))

                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado));
        }
    }
}
using AutoMapper;
using AnhApi.Esquemas;
using AnhApi.Modelos;
using AnhApi.Modelos.prm;

namespace AnhApi.Mapeos
{
    public class PerfilDepartamento:Profile
    {
        public PerfilDepartamento()
        {
            CreateMap<DepartamentoCreacion, Departamento>()
                //Ignorar la clave primaria ya que es generada por la BD
                .ForMember(dest => dest.id_departamento, opt => opt.Ignore())
                .ForMember(dest => dest.id_pais, opt => opt.MapFrom(src => src.IdPais))
                .ForMember(dest => dest.id_region_geografica, opt => opt.MapFrom(src => src.IdRegionGeografica))
                .ForMember(dest => dest.nombre_departamento, opt => opt.MapFrom(src => src.NombreDepartamento))
                .ForMember(dest => dest.abrev_int, opt => opt.MapFrom(src => src.AbrevInt))
                .ForMember(dest => dest.abrev_2, opt => opt.MapFrom(src => src.Abrev2))
                .ForMember(dest => dest.abrev_3, opt => opt.MapFrom(src => src.Abrev3))
                // Ignorar campos de auditoría, se manejarán en la capa de servicio
                .ForMember(dest => dest.aud_estado, opt => opt.Ignore());

            // Mapeo de Modelo de Dominio a DTO de Listado
            // Cuando obtienes Departamentos de la DB para mostrarlos en una lista
            CreateMap<Departamento, DepartamentoListado>()
                .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.id_departamento))
                .ForMember(dest => dest.IdPais, opt => opt.MapFrom(src => src.id_pais))
                .ForMember(dest => dest.NombreDepartamento, opt => opt.MapFrom(src => src.nombre_departamento))
                .ForMember(dest => dest.AbrevInt, opt => opt.MapFrom(src => src.abrev_int))
                .ForMember(dest => dest.Abrev2, opt => opt.MapFrom(src => src.abrev_2))
                .ForMember(dest => dest.Abrev3, opt => opt.MapFrom(src => src.abrev_3));

            // Mapeo de Modelo de Dominio a DTO de Esquema Completo
            // Para obtener detalles de un Departamento o para su actualización (POST/PUT completo)
            CreateMap<Departamento, EsqDepartamento>()
                .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.id_departamento))
                .ForMember(dest => dest.IdPais, opt => opt.MapFrom(src => src.id_pais))
                .ForMember(dest => dest.IdRegionGeografica, opt => opt.MapFrom(src => src.id_region_geografica))
                .ForMember(dest => dest.NombreDepartamento, opt => opt.MapFrom(src => src.nombre_departamento))
                .ForMember(dest => dest.AbrevInt, opt => opt.MapFrom(src => src.abrev_int))
                .ForMember(dest => dest.Abrev2, opt => opt.MapFrom(src => src.abrev_2))
                .ForMember(dest => dest.Abrev3, opt => opt.MapFrom(src => src.abrev_3))
                // Mapear campos de auditoría si se desean exponer en el DTO completo
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado));

            // Mapeo de DTO de Esquema Completo a Modelo de Dominio (para Actualización)
            // Cuando recibes un DepartamentoEsq para actualizar un registro existente
            CreateMap<EsqDepartamento, Departamento>()
                .ForMember(dest => dest.id_departamento, opt => opt.MapFrom(src => src.IdDepartamento))
                .ForMember(dest => dest.id_pais, opt => opt.MapFrom(src => src.IdPais))
                .ForMember(dest => dest.id_region_geografica, opt => opt.MapFrom(src => src.IdRegionGeografica))
                .ForMember(dest => dest.nombre_departamento, opt => opt.MapFrom(src => src.NombreDepartamento))
                .ForMember(dest => dest.abrev_int, opt => opt.MapFrom(src => src.AbrevInt))
                .ForMember(dest => dest.abrev_2, opt => opt.MapFrom(src => src.Abrev2))
                .ForMember(dest => dest.abrev_3, opt => opt.MapFrom(src => src.Abrev3))
                // Mapear campos de auditoría desde el DTO al modelo para la actualización
                .ForMember(dest => dest.aud_estado, opt => opt.MapFrom(src => src.AudEstado));

            CreateMap<Provincia, ProvinciaListado>()
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.id_provincia))
                .ForMember(dest => dest.IdProvinciaAux, opt => opt.MapFrom(src => src.id_provincia_aux))
                .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.id_departamento))
                .ForMember(dest => dest.NombreProvincia, opt => opt.MapFrom(src => src.nombre_provincia));

            CreateMap<Departamento, DeptoConProvinciasEsq>()
                .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.id_departamento))
                .ForMember(dest => dest.IdPais, opt => opt.MapFrom(src => src.id_pais))
                .ForMember(dest => dest.IdRegionGeografica, opt => opt.MapFrom(src => src.id_region_geografica))
                .ForMember(dest => dest.NombreDepartamento, opt => opt.MapFrom(src => src.nombre_departamento))
                .ForMember(dest => dest.AbrevInt, opt => opt.MapFrom(src => src.abrev_int))
                .ForMember(dest => dest.Abrev2, opt => opt.MapFrom(src => src.abrev_2))
                .ForMember(dest => dest.Abrev3, opt => opt.MapFrom(src => src.abrev_3))
                .ForMember(dest => dest.AudEstado, opt => opt.MapFrom(src => src.aud_estado))
                .ForMember(dest => dest.Provincias, opt => opt.MapFrom(src => src.Provincias));

        }
    }
}

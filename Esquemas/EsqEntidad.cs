using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AnhApi.Modelos;
using NetTopologySuite.Geometries;

namespace AnhApi.Esquemas
{
    #region EntidadCreacion
    /// <summary>
    /// Esquema para la creación de una nueva entidad.
    /// </summary>
    public class EntidadCreacion
    {

        public Guid? IdEntidadPadre { get; set; }

        [Required(ErrorMessage = "El tipo de entidad es requerido.")]
        public int IdTipoEntidad { get; set; }
        [Required(ErrorMessage = "El tipo de sociedad es requerido.")]
        public int IdTipoSociedad { get; set; }
        [Required(ErrorMessage = "El ámbito de operación es requerido.")]
        public int IdAmbitoOperacion { get; set; }
        public int? IdLocalidad { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdEstadoOperacion { get; set; }
        public int? IdEstadoEmpadronamiento { get; set; }

        [Required(ErrorMessage = "La denominación es requerida.")]
        [StringLength(250, ErrorMessage = "La denominación no puede exceder los 250 caracteres.")]
        public string Denominacion { get; set; } = null!;

        [StringLength(50, ErrorMessage = "La sigla no puede exceder los 50 caracteres.")]
        public string? Sigla { get; set; }
        [Required(ErrorMessage = "La fecha de registro es requerida.")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow; // Asignar fecha actual por defecto

        public object? Direccion { get; set; }
        public object? Telefono { get; set; }
        public object? Correo { get; set; }
        public Point? Posicion { get; set; }
    }
    #endregion

    #region EntidadListado
    /// <summary>
    /// Esquema para listar una entidad (vista reducida).
    /// </summary>
    public class EntidadListado
    {
        [Column("id_entidad")]
        public Guid Id_Entidad { get; set; }
        [Column("tipo")]
        public string Tipo { get; set; }
        [Column("sociedad")]
        public string Sociedad { get; set; }
        [Column("area")]
        public string Area { get; set; }
        [Column("localidad")]
        public string Localidad { get; set; }
        [Column("municipio")]
        public string Municipio { get; set; }
        [Column("provincia")]
        public string Provincia { get; set; }
        [Column("departamento")]
        public string Departamento { get; set; }
        [Column("denominacion")]
        public string Denominacion { get; set; }
        [Column("sigla")]
        public string Sigla { get; set; }
        [Column("identificacion")]
        public string Identificacion { get; set; }
        [Column("tipo_identificacion")]
        public string Tipo_Identificacion { get; set; }
        [Column("estado_operacion")]
        public string Estado_Operacion { get; set; }
        [Column("empadronado")]
        public string Empadronado { get; set; }
    }
    #endregion

    public class EntidadListadoRepresentante : EntidadListado
    {
        public ICollection<EsqRepresentanteEntidad> Representantes { get; set; } = new List<EsqRepresentanteEntidad>();

    }


    public class EntidadListadoDocumentos: EntidadListado
    {
        public ICollection<DocEntidadDespliegue> Documentos { get; set; } = new List<DocEntidadDespliegue>();
    }


    #region EsqEntidad
    /// <summary>
    /// Esquema completo de una entidad, usado para respuestas detalladas y actualizaciones.
    /// </summary>
    public class EsqEntidad : EntidadCreacion
    {
        [Required(ErrorMessage = "El ID de la entidad es requerido.")]
        public Guid IdEntidad { get; set; }

        // Campos de auditoría
        public int? AudEstado { get; set; }

        [Required(ErrorMessage = "El usuario de auditoría es requerido.")]
        [StringLength(30, ErrorMessage = "El usuario de auditoría no puede exceder los 30 caracteres.")]
        public string AudUsuario { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de auditoría es requerida.")]
        public DateTime AudFecha { get; set; }

        [Required(ErrorMessage = "La IP de auditoría es requerida.")]
        [StringLength(15, ErrorMessage = "La IP de auditoría no puede exceder los 15 caracteres.")]
        public string AudIp { get; set; } = null!;
    }

    public class EntidadConRep: EsqEntidad
    {
        public ICollection<Persona> Representantes { get; set; } = new List<Persona>();
    }

    #endregion
}
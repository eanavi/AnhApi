using System.Text.Json;
using NetTopologySuite.Geometries;

namespace AnhApi.Modelos
{
    /// <summary>
    /// Modelo de la tabla 'public.entidad'.
    /// </summary>
    public class Entidad : ModeloBase
    {

        public Guid id_entidad { get; set; }
        public Guid? id_entidad_padre { get; set; }
        public int id_tipo_entidad { get; set; }
        public int id_tipo_sociedad { get; set; }
        public int id_ambito_operacion { get; set; }
        public int? id_localidad { get; set; }
        public int? id_municipio { get; set; }
        public int? id_estado_operacion { get; set; }
        public int? id_estado_empadronamiento { get; set; }
        public string denominacion { get; set; } = null!;
        public string? sigla { get; set; }
        public DateTime fecha_registro { get; set; }
        public JsonDocument? direccion { get; set; }
        public JsonDocument? telefono { get; set; }
        public JsonDocument? correo { get; set; }
        public NetTopologySuite.Geometries.Point? posicion { get; set; }


        // Propiedades de navegación (Relaciones)
        /// <summary>
        /// Entidad padre a la que pertenece esta entidad.
        /// </summary>
        public Entidad? EntidadPadre { get; set; }

        /// <summary>
        /// Colección de entidades hijas que dependen de esta entidad.
        /// </summary>
        public ICollection<Entidad>? EntidadesHijas { get; set; }

        public ICollection<RepresentanteEntidad> Representantes { get; set; } = new List<RepresentanteEntidad>();
        public ICollection<DocumentoEntidad> Documentos { get; set; } = new List<DocumentoEntidad>();

    }
}
using System;
using AnhApi.Interfaces; // Asegúrate de que ModeloBase o IAuditable están aquí
using System.Collections.Generic; // Para colecciones si las hubiera

namespace AnhApi.Modelos
{
    // Hereda de ModeloBase para los campos de auditoría (aud_estado, aud_usuario, aud_ip, aud_fecha)
    public class DocumentoEntidad : ModeloBase
    {
        // Clave primaria: id_documento_entidad (serial4 en DB, mapea a int)
        public int id_documento_entidad { get; set; }

        // Clave foránea: id_entidad (uuid NOT NULL en DB)
        // Comentario: "Entidad de la que se registran los documentos para su inscripcion"
        public Guid id_entidad { get; set; }

        // Campo: tipo_doc_inscr (int4 NOT NULL en DB)
        public int tipo_doc_inscr { get; set; }

        // Campo: cite (varchar(60) NULL en DB)
        public string? cite { get; set; }

        // Campo: nombre_archivo (varchar(200) NULL en DB)
        // Comentario: "Nombre del archivo donde se guardara el documento descrito"
        public string? nombre_archivo { get; set; }

        // Campo: fecha_doc (date NOT NULL en DB)
        // Comentario: "Fecha del documento"
        public DateTime fecha_doc { get; set; } = DateTime.UtcNow; // Asignar fecha actual por defecto

        // Campo: url_archivo (varchar(200) NULL en DB)
        // Comentario: "Ubicacion del archivo donde esta almacenado el documento"
        public string? url_archivo { get; set; }

        // Campo: observaciones (varchar(259) NULL en DB)
        public string? observaciones { get; set; }

        // Propiedad de navegación para la relación con Entidad
        // Un DocumentoEntidad pertenece a una Entidad
        //public Entidad Entidad { get; set; } = null!; // Asumo que id_entidad es NOT NULL, por lo tanto la navegación es requerida
    }
}
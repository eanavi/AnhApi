namespace AnhApi.Esquemas
{
    public class RepresentanteEntidadCreacion
    {
        public Guid IdPersona { get; set; } // Identificador de la persona que es representante
        public Guid IdEntidad { get; set; } // Identificador de la entidad a la que representa
        public int TipoRepresentante { get; set; } // 1: Representante Legal, 2: Representante Administrativo, 3: Otro
    }
    public class EsqRepresentanteEntidad: RepresentanteEntidadCreacion
    {
        public int IdRepresentanteEntidad { get; set; } // Identificador único del representante de entidad
        public string? TipoRepresentante { get; set; }
        public Guid IdPersona { get; set; }
        public string NombreRepresentante { get; set; }
    }
}

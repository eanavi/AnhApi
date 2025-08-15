using AnhApi.Modelos.prm;

namespace AnhApi.Modelos
{
    public class RepresentanteEntidad : ModeloBase
    {
        public int id_representante_entidad { get; set; }
        public Guid id_persona { get; set; }
        public Guid id_entidad { get; set; }
        public int tipo_representante { get; set; } // 1: Representante Legal, 2: Representante Administrativo, 3: Otro

        public Persona? Persona { get; set; } = null!; // Relación con Persona, no puede ser nulo
        public Entidad? Entidad { get; set; } = null!; // Relación con Entidad, no puede ser nulo

        //************* ojo *************
        public Parametro? ParametroTipoRepresentante { get; set; } = null!;
    }
}

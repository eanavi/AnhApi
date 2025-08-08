using Microsoft.EntityFrameworkCore;
using AnhApi.Modelos;
using AnhApi.Modelos.prm;


namespace AnhApi.Datos
{
    public class ContextoAppBD : DbContext
    {
        public ContextoAppBD(DbContextOptions<ContextoAppBD> options) : base(options) { }


        //Inclusion de las Entidades al ORM
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Jerarquia> JerarquiasMunicipio { get; set; }
        public DbSet<DocumentoEntidad> DocumentosEntidad { get; set; }

        public DbSet<Localidad> Localidades { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Registrar todas las configuraciones en el ensamblado donde está AppDbContext
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContextoAppBD).Assembly);

        }

    }
}
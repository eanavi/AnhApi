using Microsoft.EntityFrameworkCore;
using AnhApi.Modelos;
using AnhApi.Modelos.prm;


namespace AnhApi.Datos
{
    public class ContextoAppBD : DbContext
    {
        public ContextoAppBD(DbContextOptions<ContextoAppBD> options) : base(options) { }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<Pais> Paises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Registrar todas las configuraciones en el ensamblado donde está AppDbContext
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContextoAppBD).Assembly);

        }

    }
}
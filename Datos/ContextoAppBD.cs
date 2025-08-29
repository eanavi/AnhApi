using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AnhApi.Modelos;
using AnhApi.Modelos.prm;
using AutoMapper.Execution;
using System.Drawing;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NpgsqlTypes;
using NetTopologySuite.Geometries;
using AnhApi.Esquemas;


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
        public DbSet<Entidad> Entidades { get; set; }
        public DbSet<Localidad> Localidades { get; set; }
        public DbSet<EntidadListado> EntidadesListado { get; set; }
        public DbSet<RepresentanteEntidad> RepresentantesEntidad { get; set; }
        public DbSet<Perfil> Perfiles { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Registrar todas las configuraciones en el ensamblado donde está AppDbContext
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContextoAppBD).Assembly);

            modelBuilder.Entity<EntidadListado>().HasNoKey();

        }

    }
}
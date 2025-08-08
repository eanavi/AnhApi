using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnhApi.Modelos.prm; // Para el modelo Provincia
namespace AnhApi.Datos.Configuraciones
{
    public class ConfProvincia : IEntityTypeConfiguration<Provincia>
    {
        public void Configure(EntityTypeBuilder<Provincia> entity)
        {
            entity.ToTable("provincia", "public"); 

            entity.HasKey(e => e.id_provincia)
                  .HasName("pk_id_provincia");

            entity.Property(e => e.id_provincia)
                .HasColumnName("id_provincia")
                .ValueGeneratedOnAdd(); // serial4, la BD genera el valor al añadir

            entity.Property(e => e.id_provincia_aux)
                .HasColumnName("id_provincia_aux")
                .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.id_departamento)
                .HasColumnName("id_departamento")
                .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.nombre_provincia)
                .HasColumnName("nombre_provincia")
                .HasMaxLength(100) // varchar(100)
                .IsRequired(); // NOT NULL

            // Configuración de los campos de auditoría heredados de ParametroBase
            entity.Property(e => e.aud_estado)
                .HasColumnName("aud_estado")
                .IsRequired() // int4 DEFAULT 0 NOT NULL
                .HasDefaultValue(0);

            // Configuración de la relación de clave foránea con Departamento
            entity.HasOne(d => d.Departamento)
                .WithMany(p => p.Provincias)
                .HasForeignKey(d => d.id_departamento)
                .OnDelete(DeleteBehavior.Cascade) // Comportamiento al eliminar: Cascada
                .HasConstraintName("fk_departamento_provincia"); // Nombre de la restricción FOREIGN KEY

            entity.HasMany(p => p.Municipios)
                .WithOne(m => m.Provincia)
                .HasForeignKey(m => m.id_provincia)
                .HasConstraintName("fk_provincia_municipio");

            entity.HasMany(l => l.Localidades)
                .WithOne(m => m.Provincia)
                .HasForeignKey(m => m.id_provincia)
                .HasConstraintName("fk_provincia_localidad");

        }
    }
}

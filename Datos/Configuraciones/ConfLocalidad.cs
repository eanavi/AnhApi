using AnhApi.Modelos.prm;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfLocalidad : IEntityTypeConfiguration<Localidad>
    {
        public void Configure(EntityTypeBuilder<Localidad> entity)
        {
            // Mapea la clase a la tabla 'localidad' en el esquema 'public'
            entity.ToTable("localidad", "public");

            // Configuración de la clave primaria
            entity.HasKey(e => e.id_localidad)
                  .HasName("pk_id_localidad");

            // Configuración de las propiedades
            entity.Property(e => e.id_localidad)
                  .HasColumnName("id_localidad")
                  .ValueGeneratedOnAdd(); // Para 'serial4'

            entity.Property(e => e.id_provincia)
                  .HasColumnName("id_provincia")
                  .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.nombre_localidad)
                  .HasColumnName("nombre_localidad")
                  .HasMaxLength(100) // varchar(100)
                  .IsRequired(); // NOT NULL

            // Configuración de los campos de auditoría heredados de ParametroBase
            entity.Property(e => e.aud_estado)
                  .HasColumnName("aud_estado")
                  .IsRequired()
                  .HasDefaultValue(0);

            // Configuración de la relación de clave foránea con Provincia
            entity.HasOne(l => l.Provincia) // Una Localidad tiene una Provincia
                  .WithMany(p => p.Localidades) // Una Provincia puede tener muchas Localidades (si no hay propiedad de navegación inversa en Provincia)
                  .HasForeignKey(l => l.id_provincia)
                  .HasConstraintName("fk_provincia_localidad")
                  .IsRequired(); // id_provincia es NOT NULL
        }
    }
}
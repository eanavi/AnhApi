using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnhApi.Modelos.prm;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfMunicipio : IEntityTypeConfiguration<Municipio>
    {
        public void Configure(EntityTypeBuilder<Municipio> entity)
        {
            // Mapea la clase a la tabla 'municipio' en el esquema 'public'
            entity.ToTable("municipio", "public");

            // Configuración de la clave primaria
            entity.HasKey(e => e.id_municipio)
                  .HasName("pk_id_municipio");

            // Configuración de las propiedades
            entity.Property(e => e.id_municipio)
                  .HasColumnName("id_municipio")
                  .ValueGeneratedOnAdd(); // Para 'serial4'

            entity.Property(e => e.id_provincia)
                  .HasColumnName("id_provincia")
                  .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.nombre_municipio)
                  .HasColumnName("nombre_municipio")
                  .HasMaxLength(100) // varchar(100)
                  .IsRequired(); // NOT NULL

            entity.Property(e => e.seccion)
                  .HasColumnName("seccion")
                  .HasMaxLength(50) // varchar(50)
                  .IsRequired(); // NOT NULL

            // Configuración de los campos de auditoría heredados de ModeloBase
            entity.Property(e => e.aud_estado)
                  .HasColumnName("aud_estado")
                  .IsRequired()
                  .HasDefaultValue(0);

            // Configuración de la relación de clave foránea con Provincia
            entity.HasOne(m => m.Provincia) // Un Municipio tiene una Provincia
                  .WithMany( p => p.Municipios) // Una Provincia puede tener muchos Municipios
                  .HasForeignKey(m => m.id_provincia)
                  .HasConstraintName("fk_provincia_municipio")
                  .IsRequired();


        }
    }
}
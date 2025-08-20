using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnhApi.Modelos; // Para el modelo DocumentoEntidad

namespace AnhApi.Datos.Configuraciones
{
    public class ConfDocumentoEntidad : IEntityTypeConfiguration<DocumentoEntidad>
    {
        public void Configure(EntityTypeBuilder<DocumentoEntidad> entity)
        {
            // Mapea la clase a la tabla 'documento_entidad' en el esquema 'public'
            entity.ToTable("documento_entidad", "public");

            // Configuración de la clave primaria
            entity.HasKey(e => e.id_documento_entidad)
                  .HasName("pk_id_documento_entidad");

            // Configuración de las propiedades
            entity.Property(e => e.id_documento_entidad)
                  .HasColumnName("id_documento_entidad")
                  .ValueGeneratedOnAdd(); // Para 'serial4'

            entity.Property(e => e.id_entidad)
                  .HasColumnName("id_entidad")
                  .IsRequired(); // uuid NOT NULL

            entity.Property(e => e.tipo_doc_inscr)
                  .HasColumnName("tipo_doc_inscr")
                  .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.cite)
                  .HasColumnName("cite")
                  .HasMaxLength(60); // varchar(60) NULL

            entity.Property(e => e.fecha_doc)
                .HasColumnName("fecha_doc")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.nombre_archivo)
                  .HasColumnName("nombre_archivo")
                  .HasMaxLength(200); // varchar(200) NULL

            entity.Property(e => e.url_archivo)
                  .HasColumnName("url_archivo")
                  .HasMaxLength(200); // varchar(200) NULL

            entity.Property(e => e.observaciones)
                  .HasColumnName("observaciones")
                  .HasMaxLength(259); // varchar(259) NULL

            // Configuración de los campos de auditoría heredados de ModeloBase
            entity.Property(e => e.aud_estado)
                  .HasColumnName("aud_estado")
                  .IsRequired()
                  .HasDefaultValue(0);

            entity.Property(e => e.aud_usuario)
                  .HasColumnName("aud_usuario")
                  .HasMaxLength(30)
                  .IsRequired();

            entity.Property(e => e.aud_fecha)
                  .HasColumnName("aud_fecha")
                  .IsRequired()
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.aud_ip)
                  .HasColumnName("aud_ip")
                  .HasMaxLength(15)
                  .IsRequired();
            /*
            // Configuración de la relación de clave foránea con Entidad
            entity.HasOne(de => de.Entidad) // Un DocumentoEntidad tiene una Entidad
                  .WithMany() // Una Entidad puede tener muchos DocumentoEntidad (si no hay propiedad de navegación inversa en Entidad)
                              // Si tu modelo Entidad tiene una ICollection<DocumentoEntidad> Documentos, usarías .WithMany(e => e.Documentos)
                  .HasForeignKey(de => de.id_entidad)
                  .HasConstraintName("fk_id_entidad_documento_entidad")
                  .IsRequired(); // id_entidad es NOT NULL
            */
        }
    }
}
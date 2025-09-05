using System.Text.Json;
using AnhApi.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace AnhApi.Datos.Configuraciones
{
    public class ConfPersona : IEntityTypeConfiguration<Persona>
    {
        public void Configure(EntityTypeBuilder<Persona> entity)
        {
            entity.ToTable("persona", "public");

            // Configuración de la clave primaria
            entity.HasKey(e => e.id_persona);

            // Configuración del índice único
            entity.HasIndex(e => e.numero_identificacion).IsUnique();

            // Configuración de las propiedades
            entity.Property(e => e.id_persona)
                  .HasColumnName("id_persona")
                  .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.nombre)
                  .HasColumnName("nombre")
                  .HasMaxLength(60)
                  .IsRequired();

            entity.Property(e => e.primer_apellido)
                  .HasColumnName("primer_apellido")
                  .HasMaxLength(30)
                  .IsRequired();

            entity.Property(e => e.segundo_apellido)
                  .HasColumnName("segundo_apellido")
                  .HasMaxLength(30)
                  .IsRequired();

            entity.Property(e => e.fecha_nacimiento)
                  .HasColumnName("fecha_nacimiento")
                  .IsRequired();

            entity.Property(e => e.numero_identificacion)
                  .HasColumnName("numero_identificacion")
                  .HasMaxLength(20)
                  .IsRequired();

            entity.Property(e => e.complemento)
                  .HasColumnName("complemento")
                  .HasMaxLength(10);

            entity.Property(e => e.genero)
                  .HasColumnName("genero")
                  .HasColumnType("int4");

            entity.Property(e => e.direccion)
                  .HasColumnName("direccion")
                  .HasColumnType("jsonb")
                  .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, new JsonSerializerOptions()) : null,
                    v => !string.IsNullOrEmpty(v) ? JsonDocument.Parse(v, default(JsonDocumentOptions)) : null);

            entity.Property(e => e.telefono)
                  .HasColumnName("telefono")
                  .HasColumnType("jsonb")
                  .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, new JsonSerializerOptions()) : null,
                    v => !string.IsNullOrEmpty(v) ? JsonDocument.Parse(v, default(JsonDocumentOptions)) : null
                  );

            entity.Property(e => e.correo)
                  .HasColumnName("correo")
                  .HasColumnType("jsonb")
                  .HasConversion(
                     v => v != null ? JsonSerializer.Serialize(v, new JsonSerializerOptions()) : null,
                     v => !string.IsNullOrEmpty(v) ? JsonDocument.Parse(v, default(JsonDocumentOptions)) : null
                );

            entity.Property(e => e.aud_estado)
                  .HasColumnName("aud_estado")
                  .IsRequired(true); // int4 NULL

            entity.Property(e => e.aud_usuario)
                  .HasColumnName("aud_usuario")
                  .HasMaxLength(30) // varchar(30)
                  .IsRequired(true); // varchar(30) NULL

            entity.Property(e => e.aud_fecha)
                  .HasColumnName("aud_fecha")
                  .IsRequired(true); // timestamp NULL

            entity.Property(e => e.aud_ip)
                  .HasColumnName("aud_ip")
                  .HasMaxLength(15) // varchar(45)
                  .IsRequired(true); // varchar(45) NULL

            // Una Persona (p) puede tener UN Usuario (p.Usuario)
            entity.HasOne(p => p.Usuario)
                  // El Usuario (u) está relacionado con UNA Persona (u.Persona).
                  // Aquí es donde establecemos el lado inverso de la relación.
                  .WithOne(u => u.Persona)
                  // La clave foránea 'id_persona' está en la entidad Usuario.
                  .HasForeignKey<Usuario>(u => u.id_persona)
                  // Nombre opcional para la restricción de la FK
                  .HasConstraintName("fk_persona_usuario")
                  // Esta FK es requerida porque Usuario.Persona no es nullable en tu modelo Usuario.
                  // Esto significa que CADA Usuario DEBE tener una Persona.
                  .IsRequired(); // Por defecto, si la FK es GUID y el modelo Persona es null!, es requerido.

        }
    }
}

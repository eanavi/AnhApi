using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using AnhApi.Modelos;
using NetTopologySuite.Geometries;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfEntidad : IEntityTypeConfiguration<Entidad>
    {
        public void Configure(EntityTypeBuilder<Entidad> entity)
        {
            entity.ToTable("entidad", "public");

            // Configuración de la clave primaria
            entity.HasKey(e => e.id_entidad)
                  .HasName("pk_id_entidad");

            // Configuración de las propiedades
            entity.Property(e => e.id_entidad)
                  .HasColumnName("id_entidad")
                  .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.id_entidad_padre).HasColumnName("id_entidad_padre");
            entity.Property(e => e.id_tipo_entidad).HasColumnName("id_tipo_entidad").IsRequired();
            entity.Property(e => e.id_tipo_sociedad).HasColumnName("id_tipo_sociedad").IsRequired();
            entity.Property(e => e.id_ambito_operacion).HasColumnName("id_ambito_operacion").IsRequired();
            entity.Property(e => e.id_localidad).HasColumnName("id_localidad");
            entity.Property(e => e.id_municipio).HasColumnName("id_municipio");
            entity.Property(e => e.id_estado_operacion).HasColumnName("id_estado_operacion");
            entity.Property(e => e.id_estado_empadronamiento).HasColumnName("id_estado_empadronamiento");

            entity.Property(e => e.denominacion)
                  .HasColumnName("denominacion")
                  .HasMaxLength(250)
                  .IsRequired();

            entity.Property(e => e.sigla)
                  .HasColumnName("sigla")
                  .HasMaxLength(50);

            entity.Property(e => e.identificacion)
                  .HasColumnName("identificacion")
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(e => e.tipo_identificacion)
                  .HasColumnName("tipo_identificacion")
                  .IsRequired();

            entity.Property(e => e.fecha_registro)
                  .HasColumnName("fecha_registro")
                  .IsRequired();

            // Configuración de los campos JSONB
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
                      v => !string.IsNullOrEmpty(v) ? JsonDocument.Parse(v, default(JsonDocumentOptions)) : null);

            entity.Property(e => e.correo)
                  .HasColumnName("correo")
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => v != null ? JsonSerializer.Serialize(v, new JsonSerializerOptions()) : null,
                      v => !string.IsNullOrEmpty(v) ? JsonDocument.Parse(v, default(JsonDocumentOptions)) : null);

            // Configuración de los campos de auditoría
            entity.Property(e => e.aud_estado).HasColumnName("aud_estado");
            entity.Property(e => e.aud_usuario).HasColumnName("aud_usuario").HasMaxLength(30).IsRequired();
            entity.Property(e => e.aud_fecha).HasColumnName("aud_fecha").HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
            entity.Property(e => e.aud_ip).HasColumnName("aud_ip").HasMaxLength(15).IsRequired();

            // Configuración de la relación recursiva (autorelación)
            entity.HasOne(e => e.EntidadPadre)
                  .WithMany(e => e.EntidadesHijas)
                  .HasForeignKey(e => e.id_entidad_padre)
                  .HasConstraintName("fk_id_entidad_padre")
                  .IsRequired(false); // La columna id_entidad_padre es NULLable

            entity.HasMany(e => e.Representantes)
                .WithOne(r => r.Entidad)
                .HasForeignKey(r => r.id_entidad);
        }
    }
}
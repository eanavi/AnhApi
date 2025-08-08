using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using AnhApi.Modelos;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfiguracionPersona : IEntityTypeConfiguration<Persona>
    {
        public void Configure(EntityTypeBuilder<Persona> builder)
        {
            builder.Property(p => p.direccion)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, new JsonSerializerOptions()) : null,
                    v => !string.IsNullOrEmpty(v) ? JsonDocument.Parse(v, default) : null
                );

            builder.Property(p => p.telefono)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, new JsonSerializerOptions()) : null,
                    v => !string.IsNullOrEmpty(v) ? JsonDocument.Parse(v, default) : null
                );

            builder.Property(p => p.correo)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, new JsonSerializerOptions()) : null,
                    v => !string.IsNullOrEmpty(v) ? JsonDocument.Parse(v, default) : null
                );
        }
    }
}

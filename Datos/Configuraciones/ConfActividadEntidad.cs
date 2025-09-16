using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnhApi.Modelos;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfActividadEntidad : IEntityTypeConfiguration<ActividadEntidad>
    {
        public void Configure(EntityTypeBuilder<ActividadEntidad> entity)
        {
            entity.ToTable("actividad_entidad", "public");

            // Configuración de la clave primaria
            entity.HasKey(e => e.id_actividad_entidad)
                  .HasName("pk_id_actividad_entidad");

            // Configuración de las propiedades
            entity.Property(e => e.id_actividad_entidad)
                  .HasColumnName("id_actividad_entidad")
                  .ValueGeneratedOnAdd();
            entity.Property(e => e.id_entidad).HasColumnName("id_entidad").IsRequired();
            entity.Property(e => e.id_actividad).HasColumnName("id_actividad").IsRequired();

            // Configuración de los campos de auditoría
            entity.Property(e => e.aud_estado).HasColumnName("aud_estado");
            entity.Property(e => e.aud_usuario).HasColumnName("aud_usuario").HasMaxLength(30).IsRequired();
            entity.Property(e => e.aud_fecha).HasColumnName("aud_fecha").HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
            entity.Property(e => e.aud_ip).HasColumnName("aud_ip").HasMaxLength(15).IsRequired();

        }
    }
}

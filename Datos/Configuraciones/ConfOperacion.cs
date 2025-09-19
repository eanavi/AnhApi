using AnhApi.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfOperacion : IEntityTypeConfiguration<Operacion>
    {
        public void Configure(EntityTypeBuilder<Operacion> entidad)
        {
            entidad.ToTable("operacion", "public");
            entidad.HasKey(e => e.id_operacion)
                   .HasName("pk_id_operacion");

            entidad.Property(e => e.id_operacion)
                .HasColumnName("id_operacion")
                .ValueGeneratedOnAdd();

            entidad.Property(e => e.nombre_operacion)
                .HasColumnName("nombre_operacion")
                .HasMaxLength(80)
                .IsRequired(true);

            entidad.Property(e => e.prefijo)
                .HasColumnName("prefijo")
                .HasMaxLength(10)
                .IsRequired(true);

            entidad.Property(e => e.prefijo_nombre)
                .HasColumnName("prefijo_nombre")
                .HasMaxLength(90)
                .IsRequired(true);

            entidad.Property(e => e.desde)
                .HasColumnName("desde")
                .IsRequired(true);

            entidad.Property(e => e.hasta)
                .HasColumnName("hasta")
                .IsRequired(false);

            entidad.Property(e => e.id_anterior)
                .HasColumnName("id_anterior")
                .IsRequired(false);

            entidad.Property(e => e.id_operacion_padre)
                .HasColumnName("id_operacion_padre")
                .IsRequired(false);

            entidad.Property(e => e.id_correlativo_config)
                .HasColumnName("id_correlativo_config")
                .IsRequired(false);

            entidad.Property(e => e.nivel)
                .HasColumnName("nivel")
                .IsRequired(false);

            entidad.Property(e => e.aud_estado)
                   .HasColumnName("aud_estado")
                   .IsRequired(true); // int4 NULL

            entidad.Property(e => e.aud_usuario)
                   .HasColumnName("aud_usuario")
                   .HasMaxLength(30) // varchar(30)
                   .IsRequired(true); // varchar(30) NULL

            entidad.Property(e => e.aud_fecha)
                  .HasColumnName("aud_fecha")
                  .IsRequired(true); // timestamp NULL

            entidad.Property(e => e.aud_ip)
                  .HasColumnName("aud_ip")
                  .HasMaxLength(15) // varchar(45)
                  .IsRequired(true); // varchar(45) NULL
        }
    }
}

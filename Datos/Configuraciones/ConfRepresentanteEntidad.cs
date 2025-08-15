using AnhApi.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfRepresentanteEntidad : IEntityTypeConfiguration<RepresentanteEntidad>
    {
        public void Configure(EntityTypeBuilder<RepresentanteEntidad> builder)
        {
            builder.ToTable("representante_entidad", "public");

            builder.HasKey(e => e.id_representante_entidad)
                   .HasName("pk_id_representante_entidad");

            builder.Property(e => e.id_representante_entidad)
                   .HasColumnName("id_representante_entidad")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.id_persona)
                   .HasColumnName("id_persona")
                   .IsRequired();

            builder.Property(e => e.id_entidad)
                   .HasColumnName("id_entidad")
                   .IsRequired();

            builder.Property(e => e.tipo_representante)
                   .HasColumnName("tipo_representante")
                   .IsRequired();

            builder.Property(e => e.aud_estado)
                   .HasColumnName("aud_estado")
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(e => e.aud_usuario)
                   .HasColumnName("aud_usuario")
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(e => e.aud_fecha)
                   .HasColumnName("aud_fecha")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .IsRequired();

            builder.Property(e => e.aud_ip)
                   .HasColumnName("aud_ip")
                   .HasMaxLength(15)
                   .IsRequired();

            // FK a Persona
            builder.HasOne(e => e.Persona)
                   .WithMany()
                   .HasForeignKey(e => e.id_persona)
                   .HasConstraintName("fk_id_persona_representante_entidad");

            // FK a Entidad
            builder.HasOne(e => e.Entidad)
                   .WithMany(e => e.Representantes)
                   .HasForeignKey(e => e.id_entidad)
                   .HasConstraintName("fk_id_entidad_representante_entidad");

            // FK a Parametro (tipo_representante)
            builder.HasOne(e => e.ParametroTipoRepresentante)
                   .WithMany(p => p.RepresentantesEntidadParametro)
                   .HasForeignKey(e => e.tipo_representante)
                   .HasConstraintName("fk_tipo_representante_parametro");
        }
    }
}

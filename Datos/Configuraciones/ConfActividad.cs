using AnhApi.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfActividad : IEntityTypeConfiguration<Actividad>
    {
        public void Configure(EntityTypeBuilder<Actividad> entidad)
        {
            entidad.ToTable("actividad", "public");

            entidad.HasKey(e => e.id_actividad)
                  .HasName("pk_id_actividad");

            entidad.Property(e => e.id_actividad)
                  .HasColumnName("id_actividad")
                  .ValueGeneratedOnAdd();

            entidad.Property(e => e.id_categoria_actividad)
                  .HasColumnName("id_categoria_actividad")
                  .IsRequired();

            entidad.Property(e => e.id_operacion)
                  .HasColumnName("id_operacion")
                  .IsRequired();

            entidad.Property(e => e.id_organigrama)
                  .HasColumnName("id_organigrama")
                  .IsRequired();

            entidad.Property(e => e.nombre_actividad)
                  .HasColumnName("nombre_actividad")
                  .HasMaxLength(100)
                  .IsRequired();

            entidad.Property(e => e.tipo_licencia)
                  .HasColumnName("tipo_licencia");

            entidad.Property(e => e.mnemonico)
                  .HasColumnName("mnemonico")
                  .HasMaxLength(20);

            //Configuracion de campos de auditoria

            entidad.Property(e => e.aud_estado)
                  .HasColumnName("aud_estado")
                  .IsRequired();

            entidad.Property(e => e.aud_usuario)
                  .HasColumnName("aud_usuario")
                  .HasMaxLength(30)
                  .IsRequired();

            entidad.Property(e => e.aud_fecha)
                  .HasColumnName("aud_fecha")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .IsRequired();

            entidad.Property(e => e.aud_ip)
                  .HasColumnName("aud_ip")
                  .HasMaxLength(15)
                  .IsRequired();

            //COnfiguracion de relaciones

/*
            entidad.HasOne(e => e.CategoriaActividad)
                  .WithMany()
                  .HasForeignKey(e => e.id_categoria_actividad)
                  .HasConstraintName("fk_id_categoria_actividad_actividad");

            entidad.HasOne(e => e.Operacion)
                  .WithMany()
                  .HasForeignKey(e => e.id_operacion)
                  .HasConstraintName("fk_id_operacion_atividad");

            entidad.HasOne(e => e.Organigrama)
                  .WithMany()
                  .HasForeignKey(e => e.id_organigrama)
                  .HasConstraintName("fk_id_organigrama");
  */

        }
    }
}
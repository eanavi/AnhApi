using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AnhApi.Modelos;


namespace AnhApi.Datos.Configuraciones
{
    public class ConfOrganigrama: IEntityTypeConfiguration<Organigrama>
    {
        public void Configure(EntityTypeBuilder<Organigrama> entidad)
        {
            entidad.ToTable("organigrama", "public");

            entidad.HasKey(e => e.id_organigrama)
                   .HasName("pk_id_organigrama");

            entidad.Property(e => e.id_organigrama)
                .HasColumnName("id_organigrama")
                .ValueGeneratedOnAdd();

            entidad.Property(e => e.nombre_organigrama)
                .HasColumnName("nombre_organigrama")
                .HasMaxLength(80)
                .IsRequired();

            entidad.Property(e => e.sigla)
                .HasColumnName("sigla")
                .HasMaxLength(20)
                .IsRequired(true);

            entidad.Property( e => e.nivel)
                .HasColumnName("nivel")
                .IsRequired(false);

            entidad.Property( e => e.tipo_organigrama)
                .HasColumnName("tipo_organigrama")
                .IsRequired(false);

            entidad.Property( e => e.clase_organigrama)
                .HasColumnName("clase_organigrama")
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

            entidad.HasOne(e => e.OrganigramaPadre)
                .WithMany(e => e.Dependientes)
                .HasForeignKey(r => r.id_organigrama_padre)
                .HasConstraintName("fk_id_organigrama_padre")
                .IsRequired(false);
        }
    }
}

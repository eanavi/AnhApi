using AnhApi.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Triangulate;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfCategoriaActividad : IEntityTypeConfiguration<CategoriaActividad>
    {
        public void Configure(EntityTypeBuilder<CategoriaActividad> entidad)
        {
            entidad.ToTable("categoria_actividad", "public");

            entidad.HasKey(e => e.id_categoria_actividad)
                   .HasName("pk_id_categoria_entidad");

            entidad.Property(e => e.id_categoria_actividad)
                .HasColumnName("id_categoria_actividad")
                .ValueGeneratedOnAdd();

            entidad.Property(e => e.nombre_categoria_actividad)
                .HasColumnName("nombre_categoria_actividad")
                .HasMaxLength(80)
                .IsRequired();

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

            entidad.HasOne(e => e.CategoriaPadre)
                .WithMany(e => e.Hijos)
                .HasForeignKey(r => r.id_padre_categoria_actividad)
                .HasConstraintName("fk_id_padre_categoria_actividad")
                .IsRequired(false);
        }
    }
}

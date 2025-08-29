using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnhApi.Modelos;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfPerfil : IEntityTypeConfiguration<Perfil>
    {
        public void Configure(EntityTypeBuilder<Perfil> entity)
        {
            entity.ToTable("perfil", "public");
            // Configuración de la clave primaria
            entity.HasKey(e => e.IdPerfil);
            // Configuración de las propiedades

            entity.Property(e => e.IdPerfil)
                  .HasColumnName("id_perfil")
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.Descripcion)
                  .HasColumnName("descripcion")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.Area)
                  .HasColumnName("area")
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(e => e.Id_Unidad)
                  .HasColumnName("id_unidad")
                  .IsRequired();

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

        }
    }
}
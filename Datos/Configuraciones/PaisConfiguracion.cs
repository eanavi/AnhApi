using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnhApi.Modelos.prm;

namespace AnhApi.Datos.Configuraciones
{
    public class PaisConfiguracion : IEntityTypeConfiguration<Pais>
    {
        public void Configure(EntityTypeBuilder<Pais> entity)
        {
            // Mapea la clase a la tabla 'pais' en el esquema 'public'
            entity.ToTable("pais", "public");

            // Configuración de la clave primaria
            entity.HasKey(e => e.id_pais)
                  .HasName("pk_id_pais"); // Opcional: Nombre de la restricción PRIMARY KEY en la DB

            // Configuración de las propiedades
            entity.Property(e => e.id_pais)
                  .HasColumnName("id_pais")
                  .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.nombre_pais)
                  .HasColumnName("nombre_pais")
                  .HasMaxLength(200) // varchar(200)
                  .IsRequired(); // NOT NULL

            entity.Property(e => e.abreviacion2)
                  .HasColumnName("abreviacion2")
                  .HasMaxLength(2) // varchar(2)
                  .IsRequired(); // NOT NULL

            entity.Property(e => e.abreviacion3)
                  .HasColumnName("abreviacion3")
                  .HasMaxLength(3) // varchar(3)
                  .IsRequired(); // NOT NULL

            entity.Property(e => e.codigo_internacional)
                  .HasColumnName("codigo_internacional")
                  .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.aud_estado)
                  .HasColumnName("aud_estado")
                  .IsRequired()
                  .HasDefaultValue(0); // DEFAULT 0 NOT NULL

        }
    }
}
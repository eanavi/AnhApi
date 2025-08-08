using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnhApi.Modelos.prm;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfParametro : IEntityTypeConfiguration<Parametro>
    {
        public void Configure(EntityTypeBuilder<Parametro> entity)
        {
            entity.ToTable("parametro", "public"); // Mapea la clase a la tabla 'parametro' en el esquema 'public'

            // Configuración de la clave primaria
            entity.HasKey(e => e.id_parametro);

            // Configuración de las propiedades
            entity.Property(e => e.id_parametro)
                    .HasColumnName("id_parametro")
                    .ValueGeneratedOnAdd(); // Para 'serial4', indica que la BD genera el valor al añadir

            entity.Property(e => e.codigo)
                    .HasColumnName("codigo")
                    .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.descripcion)
                    .HasColumnName("descripcion")
                    .HasMaxLength(250) // varchar(250)
                    .IsRequired(); // NOT NULL

            entity.Property(e => e.sigla)
                    .HasColumnName("sigla")
                    .HasMaxLength(10); // varchar(10), es NULLable por defecto en EF Core si no se usa IsRequired()

            entity.Property(e => e.grupo)
                    .HasColumnName("grupo")
                    .HasMaxLength(60) // varchar(60)
                    .IsRequired(); // NOT NULL

            entity.Property(e => e.aud_estado)
                    .HasColumnName("aud_estado")
                    .IsRequired(true) // int4 DEFAULT 0 NULL -> Es NULLable en la BD
                    .HasDefaultValue(0); // Coincide con el DEFAULT 0 de la tabla
        }
    }
}
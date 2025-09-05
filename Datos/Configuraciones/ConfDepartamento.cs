using AnhApi.Modelos; // Para el modelo Departamento
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnhApi.Datos.Configuraciones
{
    public class ConfDepartamento : IEntityTypeConfiguration<Departamento>
    {
        public void Configure(EntityTypeBuilder<Departamento> entity)
        {
            // Mapea la clase a la tabla 'departamento' en el esquema 'public'
            entity.ToTable("departamento", "public");

            // Configuración de la clave primaria
            entity.HasKey(e => e.id_departamento)
                  .HasName("pk_id_departamento"); // Nombre de la restricción PRIMARY KEY en la DB

            // Configuración de las propiedades
            entity.Property(e => e.id_departamento)
                  .HasColumnName("id_departamento")
                  .ValueGeneratedOnAdd(); // Para 'serial4', indica que la BD genera el valor al añadir

            entity.Property(e => e.id_pais)
                  .HasColumnName("id_pais")
                  .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.id_region_geografica)
                  .HasColumnName("id_region_geografica")
                  .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.nombre_departamento)
                  .HasColumnName("nombre_departamento")
                  .HasMaxLength(50) // varchar(50)
                  .IsRequired(); // NOT NULL

            entity.Property(e => e.abrev_int)
                  .HasColumnName("abrev_int")
                  .HasMaxLength(1) // bpchar(1)
                  .IsRequired(); // NOT NULL

            entity.Property(e => e.abrev_2)
                  .HasColumnName("abrev_2")
                  .HasMaxLength(2) // varchar(2)
                  .IsRequired(); // NOT NULL

            entity.Property(e => e.abrev_3)
                  .HasColumnName("abrev_3")
                  .HasMaxLength(3) // varchar(3)
                  .IsRequired(); // NOT NULL

            // Configuración de los campos de auditoría heredados de ModeloBase
            // Asumiendo que estos campos existen en ModeloBase y que son NOT NULL en la tabla 'departamento'
            entity.Property(e => e.aud_estado)
                  .HasColumnName("aud_estado")
                  .IsRequired() // int4 DEFAULT 0 NOT NULL
                  .HasDefaultValue(0);

            // Configuración de la relación de clave foránea con Pais
            // Un Departamento tiene un Pais (HasOne)
            entity.HasOne(d => d.Pais)
                  // Un Pais puede tener muchos Departamentos (WithMany)
                  .WithMany(p => p.Departamentos)
                  .HasForeignKey(d => d.id_pais) // La clave foránea es 'id_pais' en la tabla 'departamento'
                  .HasConstraintName("fk_pais_departamento") // Nombre de la restricción FK en la DB
                  .IsRequired(); // id_pais es NOT NULL en la tabla departamento
        }
    }
}

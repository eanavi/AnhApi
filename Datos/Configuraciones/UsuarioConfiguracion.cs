using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnhApi.Modelos; // Asegúrate de que este using esté presente

namespace AnhApi.Datos.Configuraciones
{
    /// <summary>
    /// Configuración de la entidad Usuario para Entity Framework Core.
    /// </summary>
    public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> entity)
        {
            // Mapea la entidad Usuario a la tabla "usuario" en el esquema "public"
            entity.ToTable("usuario", "public");

            // Configura la clave primaria de la tabla
            entity.HasKey(e => e.id_usuario);

            // Configura un índice único para la columna 'login'
            entity.HasIndex(e => e.login).IsUnique();

            // Configuración de las propiedades de la entidad Usuario
            entity.Property(e => e.id_usuario)
                  .HasColumnName("id_usuario")
                  .HasColumnType("numeric(10)"); // Asegura el mapeo a numeric(10)

            entity.Property(e => e.id_persona)
                  .HasColumnName("id_persona")
                  .IsRequired(); // uuid NOT NULL

            entity.Property(e => e.login)
                  .HasColumnName("login")
                  .HasMaxLength(50)
                  .IsRequired(); // varchar(50) NOT NULL

            entity.Property(e => e.clave)
                  .HasColumnName("clave")
                  .HasMaxLength(100)
                  .IsRequired(); // varchar(100) NOT NULL

            // Configuración de las propiedades heredadas de ModeloBase
            // Coincidiendo con la DDL de PostgreSQL:
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
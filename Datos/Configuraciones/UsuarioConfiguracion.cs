using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnhApi.Modelos; // Asegúrate de que este using apunte a tu clase Usuario del modelo

namespace AnhApi.Datos.Configuraciones
{
    public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> entity)
        {
            entity.ToTable("usuario", "public"); // Mapea la clase a la tabla 'usuario' en el esquema 'public'

            // Configuración de la clave primaria
            entity.HasKey(e => e.id_usuario);

            // Configuración del índice único para 'login'
            entity.HasIndex(e => e.login).IsUnique();

            // Configuración de las propiedades
            entity.Property(e => e.id_usuario)
                    .HasColumnName("id_usuario")
                    .ValueGeneratedOnAdd(); // Para 'serial4', indica que la BD genera el valor al añadir

            entity.Property(e => e.id_persona)
                    .HasColumnName("id_persona")
                    .IsRequired(); // uuid NOT NULL

            entity.Property(e => e.id_perfil)
                    .HasColumnName("id_perfil")
                    .IsRequired(); // int4 NOT NULL

            entity.Property(e => e.login)
                    .HasColumnName("login")
                    .HasMaxLength(50) // varchar(50)
                    .IsRequired(); // NOT NULL

            entity.Property(e => e.clave)
                    .HasColumnName("clave")
                    .HasMaxLength(100) // varchar(100)
                    .IsRequired(); // NOT NULL

            entity.Property(e => e.aud_estado)
                    .HasColumnName("aud_estado")
                    .IsRequired()
                    .HasDefaultValue(0);

            entity.Property(e => e.aud_usuario)
                    .HasColumnName("aud_usuario")
                    .HasMaxLength(30) // varchar(30)
                    .IsRequired(); // NOT NULL

            entity.Property(e => e.aud_fecha)
                    .HasColumnName("aud_fecha")
                    .IsRequired() // timestamp DEFAULT CURRENT_TIMESTAMP NULL -> Es NULLable en la BD
                    .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Coincide con el DEFAULT CURRENT_TIMESTAMP de la tabla

            entity.Property(e => e.aud_ip)
                    .HasColumnName("aud_ip")
                    .HasMaxLength(15) // varchar(15)
                    .IsRequired(); // NOT NULL

            // Si tienes una relación con Persona, puedes configurarla aquí
            entity.HasOne(u => u.Persona) // Asumiendo que tienes una propiedad 'public Persona Persona { get; set; }' en Usuario
                   .WithMany() // O .WithOne(p => p.Usuario) si la relación es 1 a 1
                   .HasForeignKey(u => u.id_persona);
        }
    }
}
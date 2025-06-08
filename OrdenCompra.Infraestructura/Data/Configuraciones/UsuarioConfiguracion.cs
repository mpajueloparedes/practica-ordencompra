using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrdenCompra.Core.Entidades;

namespace OrdenCompra.Infraestructura.Data.Configuraciones;

public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(u => u.NombreUsuario)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Usuario");

        builder.Property(u => u.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(u => u.EstaActivo)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices únicos
        builder.HasIndex(u => u.NombreUsuario)
            .IsUnique()
            .HasDatabaseName("IX_Usuarios_NombreUsuario");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Usuarios_Email");

        // Relaciones
        builder.HasMany(u => u.Ordenes)
            .WithOne(o => o.Usuario)
            .HasForeignKey(o => o.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
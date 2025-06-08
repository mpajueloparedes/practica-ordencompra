using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrdenCompra.Core.Entidades;

namespace OrdenCompra.Infraestructura.Data.Configuraciones;

public class OrdenConfiguracion : IEntityTypeConfiguration<Orden>
{
    public void Configure(EntityTypeBuilder<Orden> builder)
    {
        builder.ToTable("Ordenes");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(o => o.Cliente)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Total)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(o => o.UsuarioId)
            .IsRequired();

        // Índices
        builder.HasIndex(o => o.Cliente)
            .HasDatabaseName("IX_Ordenes_Cliente");

        builder.HasIndex(o => o.FechaCreacion)
            .HasDatabaseName("IX_Ordenes_FechaCreacion");

        builder.HasIndex(o => o.UsuarioId)
            .HasDatabaseName("IX_Ordenes_UsuarioId");

        // Índice único para evitar duplicados (mismo cliente y fecha)
        builder.HasIndex(o => new { o.Cliente, o.FechaCreacion })
            .IsUnique()
            .HasDatabaseName("IX_Ordenes_Cliente_Fecha")
            .HasFilter("DATEPART(year, FechaCreacion) = DATEPART(year, GETDATE()) AND DATEPART(dayofyear, FechaCreacion) = DATEPART(dayofyear, GETDATE())");

        // Restricciones
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Ordenes_Total_Positivo", "Total >= 0");
        });

        // Relaciones
        builder.HasOne(o => o.Usuario)
            .WithMany(u => u.Ordenes)
            .HasForeignKey(o => o.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Detalles)
            .WithOne(d => d.Orden)
            .HasForeignKey(d => d.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
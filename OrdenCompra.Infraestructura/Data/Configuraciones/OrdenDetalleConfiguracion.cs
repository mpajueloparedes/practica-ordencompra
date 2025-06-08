using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrdenCompra.Core.Entidades;

namespace OrdenCompra.Infraestructura.Data.Configuraciones;

public class OrdenDetalleConfiguracion : IEntityTypeConfiguration<OrdenDetalle>
{
    public void Configure(EntityTypeBuilder<OrdenDetalle> builder)
    {
        builder.ToTable("OrdenDetalles");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.OrdenId)
            .IsRequired();

        builder.Property(d => d.Producto)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Cantidad)
            .IsRequired();

        builder.Property(d => d.PrecioUnitario)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(d => d.Subtotal)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Índices
        builder.HasIndex(d => d.OrdenId)
            .HasDatabaseName("IX_OrdenDetalles_OrdenId");

        builder.HasIndex(d => d.Producto)
            .HasDatabaseName("IX_OrdenDetalles_Producto");

        // Restricciones
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_OrdenDetalles_Cantidad_Positiva", "Cantidad > 0");
            t.HasCheckConstraint("CK_OrdenDetalles_PrecioUnitario_Positivo", "PrecioUnitario >= 0");
            t.HasCheckConstraint("CK_OrdenDetalles_Subtotal_Positivo", "Subtotal >= 0");
        });

        // Relaciones
        builder.HasOne(d => d.Orden)
            .WithMany(o => o.Detalles)
            .HasForeignKey(d => d.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using OrdenCompra.Core.Entidades;
using OrdenCompra.Core.Excepciones;
using Xunit;

namespace OrdenCompra.Simple;

public class LogicaNegocioTests
{
    [Fact]
    public void Orden_AgregarDetalle_DebeActualizarTotal()
    {
        // Arrange
        var orden = new Orden { Cliente = "Test", UsuarioId = 1 };
        var detalle = new OrdenDetalle
        {
            Producto = "Test",
            Cantidad = 2,
            PrecioUnitario = 25.00m
        };
        detalle.CalcularSubtotal();

        // Act
        orden.AgregarDetalle(detalle);

        // Assert
        Assert.Equal(50.00m, orden.Total);
        Assert.Single(orden.Detalles);
    }

    [Fact]
    public void Orden_RemoverDetalle_DebeActualizarTotal()
    {
        // Arrange
        var orden = new Orden { Cliente = "Test", UsuarioId = 1 };
        var detalle1 = new OrdenDetalle { Producto = "P1", Cantidad = 1, PrecioUnitario = 10m };
        var detalle2 = new OrdenDetalle { Producto = "P2", Cantidad = 1, PrecioUnitario = 20m };

        detalle1.CalcularSubtotal();
        detalle2.CalcularSubtotal();

        orden.AgregarDetalle(detalle1);
        orden.AgregarDetalle(detalle2);

        // Act
        orden.RemoverDetalle(detalle1);

        // Assert
        Assert.Equal(20.00m, orden.Total);
        Assert.Single(orden.Detalles);
    }

    [Fact]
    public void OrdenDetalle_ActualizarCantidad_DebeRecalcularSubtotal()
    {
        // Arrange
        var detalle = new OrdenDetalle
        {
            Producto = "Test",
            Cantidad = 2,
            PrecioUnitario = 15.00m
        };
        detalle.CalcularSubtotal();
        Assert.Equal(30.00m, detalle.Subtotal);

        // Act
        detalle.ActualizarCantidad(3);

        // Assert
        Assert.Equal(3, detalle.Cantidad);
        Assert.Equal(45.00m, detalle.Subtotal);
    }

    [Fact]
    public void OrdenDetalle_ActualizarCantidadInvalida_DebeLanzarExcepcion()
    {
        // Arrange
        var detalle = new OrdenDetalle
        {
            Producto = "Test",
            Cantidad = 2,
            PrecioUnitario = 15.00m
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => detalle.ActualizarCantidad(0));
        Assert.Throws<ArgumentException>(() => detalle.ActualizarCantidad(-1));
    }

    [Fact]
    public void OrdenDetalle_ActualizarPrecioNegativo_DebeLanzarExcepcion()
    {
        // Arrange
        var detalle = new OrdenDetalle
        {
            Producto = "Test",
            Cantidad = 2,
            PrecioUnitario = 15.00m
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => detalle.ActualizarPrecio(-10.00m));
    }
}
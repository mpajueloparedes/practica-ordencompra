using OrdenCompra.Core.Entidades;
using OrdenCompra.Tests.Helpers;
using Xunit;

namespace OrdenCompra.Tests;

public class EntidadesTests
{
    [Fact]
    public void Orden_CalcularTotal_DebeCalcularCorrectamente()
    {
        // Arrange
        var orden = new Orden
        {
            Cliente = "Test Cliente",
            UsuarioId = 1
        };

        var detalle1 = new OrdenDetalle
        {
            Producto = "Producto 1",
            Cantidad = 2,
            PrecioUnitario = 10.00m
        };
        detalle1.CalcularSubtotal();

        var detalle2 = new OrdenDetalle
        {
            Producto = "Producto 2",
            Cantidad = 1,
            PrecioUnitario = 30.00m
        };
        detalle2.CalcularSubtotal();

        // Act
        orden.AgregarDetalle(detalle1);
        orden.AgregarDetalle(detalle2);

        // Assert
        Assert.Equal(50.00m, orden.Total);
        Assert.Equal(2, orden.Detalles.Count);
    }

    [Fact]
    public void OrdenDetalle_CalcularSubtotal_DebeCalcularCorrectamente()
    {
        // Arrange
        var detalle = new OrdenDetalle
        {
            Producto = "Test Producto",
            Cantidad = 3,
            PrecioUnitario = 15.50m
        };

        // Act
        detalle.CalcularSubtotal();

        // Assert
        Assert.Equal(46.50m, detalle.Subtotal);
    }

    [Fact]
    public void Orden_TieneDetalles_DebeRetornarVerdaderoConDetalles()
    {
        // Arrange
        var orden = TestHelper.CrearOrden();

        // Act & Assert
        Assert.True(orden.TieneDetalles());
    }

    [Fact]
    public void Usuario_CrearUsuario_DebeTenerValoresPorDefecto()
    {
        // Arrange & Act
        var usuario = new Usuario
        {
            NombreUsuario = "test",
            Email = "test@test.com",
            PasswordHash = "hash"
        };

        // Assert
        Assert.Equal("Usuario", usuario.Rol);
        Assert.True(usuario.EstaActivo);
        Assert.True(usuario.FechaCreacion > DateTime.MinValue);
    }

    [Theory]
    [InlineData(1, 10.00, 10.00)]
    [InlineData(2, 15.50, 31.00)]
    [InlineData(5, 7.25, 36.25)]
    public void OrdenDetalle_CalcularSubtotal_ConDiferentesCantidades(int cantidad, decimal precio, decimal esperado)
    {
        // Arrange
        var detalle = new OrdenDetalle
        {
            Cantidad = cantidad,
            PrecioUnitario = precio
        };

        // Act
        detalle.CalcularSubtotal();

        // Assert
        Assert.Equal(esperado, detalle.Subtotal);
    }
}

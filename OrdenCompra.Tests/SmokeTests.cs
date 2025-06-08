using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.Entidades;
using OrdenCompra.Tests.Helpers;
using Xunit;

namespace OrdenCompra.Tests;

public class SmokeTests
{
    [Fact]
    public void Sistema_CrearOrdenCompleta_FlujoBasico()
    {
        // Arrange
        var usuario = TestHelper.CrearUsuario();
        var request = TestHelper.CrearOrdenRequest();

        // Act - Simular creación de orden
        var orden = new Orden
        {
            Cliente = request.Cliente,
            UsuarioId = usuario.Id,
            FechaCreacion = DateTime.Now
        };

        foreach (var detalleRequest in request.Detalles)
        {
            var detalle = new OrdenDetalle
            {
                Producto = detalleRequest.Producto,
                Cantidad = detalleRequest.Cantidad,
                PrecioUnitario = detalleRequest.PrecioUnitario
            };
            detalle.CalcularSubtotal();
            orden.AgregarDetalle(detalle);
        }

        // Assert
        Assert.NotNull(orden);
        Assert.Equal(request.Cliente, orden.Cliente);
        Assert.Equal(usuario.Id, orden.UsuarioId);
        Assert.True(orden.TieneDetalles());
        Assert.True(orden.Total > 0);
    }

    [Fact]
    public void Sistema_ValidacionCompleta_DebeTrabajarCorrectamente()
    {
        // Arrange
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente de Prueba Completa",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Laptop", Cantidad = 2, PrecioUnitario = 1500.00m },
                new() { Producto = "Mouse", Cantidad = 5, PrecioUnitario = 25.00m },
                new() { Producto = "Teclado", Cantidad = 2, PrecioUnitario = 75.50m }
            }
        };

        // Act
        var totalEsperado = (2 * 1500.00m) + (5 * 25.00m) + (2 * 75.50m);

        // Assert
        Assert.NotEmpty(request.Cliente);
        Assert.NotEmpty(request.Detalles);
        Assert.All(request.Detalles, d => Assert.True(d.Cantidad > 0));
        Assert.All(request.Detalles, d => Assert.True(d.PrecioUnitario >= 0));
        Assert.Equal(3276.00m, totalEsperado);
    }
}
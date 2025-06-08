using OrdenCompra.Core.Entidades;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace OrdenCompra.Tests;

public class PerformanceTests
{
    private readonly ITestOutputHelper _output;

    public PerformanceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Orden_CalcularTotal_ConMuchosDetalles_DebeSerRapido()
    {
        // Arrange
        var orden = new Orden { Cliente = "Test Performance", UsuarioId = 1 };
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            var detalle = new OrdenDetalle
            {
                Producto = $"Producto {i}",
                Cantidad = i + 1,
                PrecioUnitario = 10.50m
            };
            detalle.CalcularSubtotal();
            orden.AgregarDetalle(detalle);
        }

        stopwatch.Stop();

        // Assert
        Assert.Equal(1000, orden.Detalles.Count);
        Assert.True(stopwatch.ElapsedMilliseconds < 100,
            $"Operación tomó {stopwatch.ElapsedMilliseconds}ms, esperado < 100ms");

        _output.WriteLine($"Tiempo para 1000 detalles: {stopwatch.ElapsedMilliseconds}ms");
        _output.WriteLine($"Total calculado: {orden.Total:C}");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(500)]
    public void ValidacionMasiva_DebeSerRapida(int cantidad)
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < cantidad; i++)
        {
            var detalle = new OrdenDetalle
            {
                Producto = $"Producto {i}",
                Cantidad = 1,
                PrecioUnitario = 10.00m
            };
            detalle.CalcularSubtotal();
        }

        stopwatch.Stop();

        // Assert
        var maxTiempoEsperado = cantidad * 0.1; // 0.1ms por validación
        Assert.True(stopwatch.ElapsedMilliseconds < maxTiempoEsperado,
            $"Validación de {cantidad} elementos tomó {stopwatch.ElapsedMilliseconds}ms");

        _output.WriteLine($"Validación de {cantidad} elementos: {stopwatch.ElapsedMilliseconds}ms");
    }
}

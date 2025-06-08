using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.Validadores;
using FluentValidation.TestHelper;
using Xunit;

namespace OrdenCompra.Tests;

public class ValidacionesTests
{
    private readonly CrearOrdenValidator _validator = new();

    [Fact]
    public void CrearOrdenValidator_ClienteVacio_DebeRetornarError()
    {
        // Arrange
        var request = new CrearOrdenRequest
        {
            Cliente = "",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Test", Cantidad = 1, PrecioUnitario = 10 }
            }
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Cliente);
    }

    [Fact]
    public void CrearOrdenValidator_SinDetalles_DebeRetornarError()
    {
        // Arrange
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Test",
            Detalles = new List<CrearOrdenDetalleRequest>()
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Detalles);
    }

    [Fact]
    public void CrearOrdenValidator_DatosValidos_NoDebeRetornarErrores()
    {
        // Arrange
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Válido",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Producto Test", Cantidad = 1, PrecioUnitario = 10.50m }
            }
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void CrearOrdenValidator_CantidadInvalida_DebeRetornarError(int cantidad)
    {
        // Arrange
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Test",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Test", Cantidad = cantidad, PrecioUnitario = 10 }
            }
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Detalles[0].Cantidad");
    }
}
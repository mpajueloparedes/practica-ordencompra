using FluentAssertions;
using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.Validadores;
using Xunit;

namespace OrdenCompra.Tests.Unit.Validadores;

public class CrearOrdenValidatorTests
{
    private readonly CrearOrdenValidator _validator;

    public CrearOrdenValidatorTests()
    {
        _validator = new CrearOrdenValidator();
    }

    [Fact]
    public void Validate_ConDatosValidos_DebeSerValido()
    {
        // Arrange
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Test",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Producto 1", Cantidad = 1, PrecioUnitario = 100.50m }
            }
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_SinCliente_DebeSerInvalido()
    {
        // Arrange
        var request = new CrearOrdenRequest
        {
            Cliente = "",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Producto 1", Cantidad = 1, PrecioUnitario = 100 }
            }
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().ContainSingle(x => x.PropertyName == "Cliente");
    }

    [Fact]
    public void Validate_SinDetalles_DebeSerInvalido()
    {
        // Arrange
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Test",
            Detalles = new List<CrearOrdenDetalleRequest>()
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().ContainSingle(x => x.PropertyName == "Detalles");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ConCantidadInvalida_DebeSerInvalido(int cantidad)
    {
        // Arrange
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Test",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Producto 1", Cantidad = cantidad, PrecioUnitario = 100 }
            }
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().ContainSingle(x => x.PropertyName.Contains("Cantidad"));
    }

    [Fact]
    public void Validate_ConPrecioNegativo_DebeSerInvalido()
    {
        // Arrange
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Test",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Producto 1", Cantidad = 1, PrecioUnitario = -10 }
            }
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().ContainSingle(x => x.PropertyName.Contains("PrecioUnitario"));
    }
}
using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.Enums;
using Xunit;

namespace OrdenCompra.Tests;

public class UtilsTests
{
    [Theory]
    [InlineData("Administrador", true)]
    [InlineData("Usuario", true)]
    [InlineData("Supervisor", true)]
    [InlineData("InvalidRole", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void RolUsuario_EsRolValido_DebeValidarCorrectamente(string? rol, bool esperado)
    {
        // Act
        var resultado = RolUsuario.EsRolValido(rol!);

        // Assert
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void FiltroOrdenesRequest_EsOrdenamientoValido_ConParametrosValidos()
    {
        // Arrange
        var filtro = new FiltroOrdenesRequest
        {
            OrdenarPor = "Cliente",
            OrdenarDireccion = "ASC"
        };

        // Act & Assert
        Assert.True(filtro.EsOrdenamientoValido());
    }

    [Fact]
    public void FiltroOrdenesRequest_EsOrdenamientoValido_ConParametrosInvalidos()
    {
        // Arrange
        var filtro = new FiltroOrdenesRequest
        {
            OrdenarPor = "CampoInvalido",
            OrdenarDireccion = "INVALID"
        };

        // Act & Assert
        Assert.False(filtro.EsOrdenamientoValido());
    }

}
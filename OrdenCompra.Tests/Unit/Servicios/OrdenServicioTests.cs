using FluentAssertions;
using Moq;
using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.Entidades;
using OrdenCompra.Core.Excepciones;
using OrdenCompra.Core.Interfaces;
using OrdenCompra.Core.Interfaces.Repositorios;
using OrdenCompra.Core.Servicios;
using Xunit;

namespace OrdenCompra.Tests.Unit.Servicios;

public class OrdenServicioTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IOrdenRepositorio> _mockOrdenRepo;
    private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
    private readonly OrdenServicio _ordenServicio;

    public OrdenServicioTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOrdenRepo = new Mock<IOrdenRepositorio>();
        _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();

        _mockUnitOfWork.Setup(x => x.Ordenes).Returns(_mockOrdenRepo.Object);
        _mockUnitOfWork.Setup(x => x.Usuarios).Returns(_mockUsuarioRepo.Object);

        _ordenServicio = new OrdenServicio(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CrearOrdenAsync_ConDatosValidos_DebeCrearOrdenExitosamente()
    {
        // Arrange
        var usuarioId = 1;
        var usuario = new Usuario { Id = usuarioId, NombreUsuario = "test", Email = "test@test.com" };
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Test",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Producto 1", Cantidad = 2, PrecioUnitario = 100.50m }
            }
        };

        _mockUsuarioRepo.Setup(x => x.ObtenerPorIdAsync(usuarioId))
            .ReturnsAsync(usuario);

        _mockOrdenRepo.Setup(x => x.ExisteOrdenParaClienteEnFechaAsync(It.IsAny<string>(), It.IsAny<DateTime>(), null))
            .ReturnsAsync(false);

        _mockOrdenRepo.Setup(x => x.ObtenerConDetallesAsync(It.IsAny<int>()))
            .ReturnsAsync(new Orden
            {
                Id = 1,
                Cliente = "Cliente Test",
                Usuario = usuario,
                Detalles = new List<OrdenDetalle>
                {
                    new() { Id = 1, Producto = "Producto 1", Cantidad = 2, PrecioUnitario = 100.50m, Subtotal = 201.00m }
                }
            });

        // Act
        var resultado = await _ordenServicio.CrearOrdenAsync(request, usuarioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Cliente.Should().Be("Cliente Test");
        resultado.Total.Should().Be(201.00m);

        _mockOrdenRepo.Verify(x => x.AgregarAsync(It.IsAny<Orden>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.GuardarCambiosAsync(), Times.Once);
    }

    [Fact]
    public async Task CrearOrdenAsync_SinDetalles_DebeLanzarExcepcion()
    {
        // Arrange
        var usuarioId = 1;
        var usuario = new Usuario { Id = usuarioId };
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Test",
            Detalles = new List<CrearOrdenDetalleRequest>()
        };

        _mockUsuarioRepo.Setup(x => x.ObtenerPorIdAsync(usuarioId))
            .ReturnsAsync(usuario);

        // Act & Assert
        await _ordenServicio.Invoking(x => x.CrearOrdenAsync(request, usuarioId))
            .Should().ThrowAsync<ReglaDeNegocioExcepcion>()
            .WithMessage("*sin detalles*");
    }

    [Fact]
    public async Task CrearOrdenAsync_ConUsuarioInexistente_DebeLanzarExcepcion()
    {
        // Arrange
        var usuarioId = 999;
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Test",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Producto 1", Cantidad = 1, PrecioUnitario = 100 }
            }
        };

        _mockUsuarioRepo.Setup(x => x.ObtenerPorIdAsync(usuarioId))
            .ReturnsAsync((Usuario?)null);

        // Act & Assert
        await _ordenServicio.Invoking(x => x.CrearOrdenAsync(request, usuarioId))
            .Should().ThrowAsync<RecursoNoEncontradoExcepcion>();
    }

    [Fact]
    public async Task CrearOrdenAsync_ConOrdenDuplicada_DebeLanzarExcepcion()
    {
        // Arrange
        var usuarioId = 1;
        var usuario = new Usuario { Id = usuarioId };
        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente Test",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Producto 1", Cantidad = 1, PrecioUnitario = 100 }
            }
        };

        _mockUsuarioRepo.Setup(x => x.ObtenerPorIdAsync(usuarioId))
            .ReturnsAsync(usuario);

        _mockOrdenRepo.Setup(x => x.ExisteOrdenParaClienteEnFechaAsync(It.IsAny<string>(), It.IsAny<DateTime>(), null))
            .ReturnsAsync(true);

        // Act & Assert
        await _ordenServicio.Invoking(x => x.CrearOrdenAsync(request, usuarioId))
            .Should().ThrowAsync<ReglaDeNegocioExcepcion>()
            .WithMessage("*duplicada*");
    }
}
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using OrdenCompra.Core.DTOs.Auth;
using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Infraestructura.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace OrdenCompra.Tests.Integration.Controllers;

public class OrdenesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrdenesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remover el DbContext existente
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Agregar DbContext en memoria para las pruebas
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDB");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetOrdenes_SinAutenticacion_DebeRetornarUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/ordenes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostOrden_ConDatosValidos_DebeCrearOrden()
    {
        // Arrange
        var token = await ObtenerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CrearOrdenRequest
        {
            Cliente = "Cliente de Prueba",
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Producto Test", Cantidad = 2, PrecioUnitario = 50.00m }
            }
        };

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/ordenes", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        var orden = JsonConvert.DeserializeObject<dynamic>(responseContent);

        ((string?)orden?.cliente).Should().Be("Cliente de Prueba");
        ((decimal)orden?.total).Should().Be(100.00m);
    }

    private async Task<string> ObtenerTokenAsync()
    {
        var loginRequest = new LoginRequest
        {
            NombreUsuario = "admin",
            Password = "admin123"
        };

        var json = JsonConvert.SerializeObject(loginRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/login", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

        return loginResponse?.Token ?? string.Empty;
    }
}
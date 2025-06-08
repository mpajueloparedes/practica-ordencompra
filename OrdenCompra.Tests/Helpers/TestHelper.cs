using OrdenCompra.Core.Entidades;
using OrdenCompra.Core.DTOs.Request;

namespace OrdenCompra.Tests.Helpers;

public static class TestHelper
{
    public static Usuario CrearUsuario(int id = 1, string nombre = "TestUser")
    {
        return new Usuario
        {
            Id = id,
            NombreUsuario = nombre,
            Email = $"{nombre.ToLower()}@test.com",
            PasswordHash = "hash123",
            Rol = "Usuario",
            EstaActivo = true
        };
    }

    public static Orden CrearOrden(int id = 1, string cliente = "Cliente Test")
    {
        var orden = new Orden
        {
            Id = id,
            Cliente = cliente,
            UsuarioId = 1,
            FechaCreacion = DateTime.Now
        };

        // Agregar detalle de prueba
        var detalle = new OrdenDetalle
        {
            Id = 1,
            OrdenId = id,
            Producto = "Producto Test",
            Cantidad = 2,
            PrecioUnitario = 50.00m
        };
        detalle.CalcularSubtotal();
        orden.Detalles.Add(detalle);
        orden.CalcularTotal();

        return orden;
    }

    public static CrearOrdenRequest CrearOrdenRequest(string cliente = "Cliente Test")
    {
        return new CrearOrdenRequest
        {
            Cliente = cliente,
            Detalles = new List<CrearOrdenDetalleRequest>
            {
                new() { Producto = "Producto Test", Cantidad = 2, PrecioUnitario = 50.00m }
            }
        };
    }
}
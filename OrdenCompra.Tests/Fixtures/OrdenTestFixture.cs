using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.Entidades;

namespace OrdenCompra.Tests.Fixtures;

public static class OrdenTestFixture
{
    public static Usuario CrearUsuarioTest(int id = 1, string nombre = "TestUser", string rol = "Usuario")
    {
        return new Usuario
        {
            Id = id,
            NombreUsuario = nombre,
            Email = $"{nombre.ToLower()}@test.com",
            PasswordHash = "hashedpassword",
            Rol = rol,
            FechaCreacion = DateTime.UtcNow,
            EstaActivo = true
        };
    }

    public static Orden CrearOrdenTest(int id = 1, int usuarioId = 1, string cliente = "Cliente Test")
    {
        var orden = new Orden
        {
            Id = id,
            Cliente = cliente,
            UsuarioId = usuarioId,
            FechaCreacion = DateTime.UtcNow
        };

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

    public static CrearOrdenRequest CrearOrdenRequestTest(string cliente = "Cliente Test")
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
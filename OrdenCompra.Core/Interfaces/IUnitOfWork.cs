using OrdenCompra.Core.Interfaces.Repositorios;
namespace OrdenCompra.Core.Interfaces;
public interface IUnitOfWork : IDisposable
{
    IOrdenRepositorio Ordenes { get; }
    IOrdenDetalleRepositorio OrdenDetalles { get; }
    IUsuarioRepositorio Usuarios { get; }

    Task<int> GuardarCambiosAsync();
    Task IniciarTransaccionAsync();
    Task ConfirmarTransaccionAsync();
    Task RevertirTransaccionAsync();
}
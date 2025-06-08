using OrdenCompra.Core.Entidades;

namespace OrdenCompra.Core.Interfaces.Repositorios;
public interface IUsuarioRepositorio : IBaseRepositorio<Usuario>
{
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario);
    Task<bool> ExisteEmailAsync(string email);
    Task<IEnumerable<Usuario>> ObtenerPorRolAsync(string rol);
    Task<IEnumerable<Usuario>> ObtenerActivosAsync();
    Task DesactivarUsuarioAsync(int usuarioId);
    Task ActivarUsuarioAsync(int usuarioId);
}

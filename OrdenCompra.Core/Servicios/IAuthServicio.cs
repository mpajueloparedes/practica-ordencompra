using OrdenCompra.Core.DTOs.Auth;
using OrdenCompra.Core.Entidades;

namespace OrdenCompra.Core.Interfaces.Servicios;

public interface IAuthServicio
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    string GenerarTokenJwtAsync(Usuario usuario);
    Task<bool> ValidarPasswordAsync(string password, string hash);
    Task<string> HashPasswordAsync(string password);
    Task<Usuario?> ObtenerUsuarioPorTokenAsync(string token);
    bool ValidarToken(string token);
    Task<bool> TienePermisoAsync(int usuarioId, string accion);
}

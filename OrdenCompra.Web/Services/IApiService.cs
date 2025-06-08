using OrdenCompra.Core.DTOs.Auth;
using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.DTOs.Response;

namespace OrdenCompra.Web.Services;

public interface IApiService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<ListaOrdenesResponse?> ObtenerOrdenesAsync(FiltroOrdenesRequest filtros);
    Task<OrdenResponse?> ObtenerOrdenPorIdAsync(int id);
    Task<OrdenResponse?> CrearOrdenAsync(CrearOrdenRequest request);
    Task<OrdenResponse?> ActualizarOrdenAsync(ActualizarOrdenRequest request);
    Task<bool> EliminarOrdenAsync(int id);
    Task<IEnumerable<OrdenResponse>?> ObtenerMisOrdenesAsync();
    void EstablecerToken(string token);
    void LimpiarToken();
}

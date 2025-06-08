using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.DTOs.Response;

namespace OrdenCompra.Core.Interfaces.Servicios;

public interface IOrdenServicio
{
    Task<OrdenResponse> CrearOrdenAsync(CrearOrdenRequest request, int usuarioId);
    Task<OrdenResponse> ActualizarOrdenAsync(ActualizarOrdenRequest request, int usuarioId);
    Task EliminarOrdenAsync(int ordenId, int usuarioId);
    Task<OrdenResponse?> ObtenerOrdenPorIdAsync(int ordenId);
    Task<ListaOrdenesResponse> ObtenerOrdenesAsync(FiltroOrdenesRequest filtros);
    Task<IEnumerable<OrdenResponse>> ObtenerOrdenesPorUsuarioAsync(int usuarioId);
    Task<bool> PuedeEliminarOrdenAsync(int ordenId, int usuarioId);
    Task<bool> PuedeModificarOrdenAsync(int ordenId, int usuarioId);
}

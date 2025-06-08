using OrdenCompra.Core.Entidades;
namespace OrdenCompra.Core.Interfaces.Repositorios;
public interface IOrdenDetalleRepositorio : IBaseRepositorio<OrdenDetalle>
{
    Task<IEnumerable<OrdenDetalle>> ObtenerPorOrdenIdAsync(int ordenId);
    Task<OrdenDetalle?> ObtenerPorOrdenYProductoAsync(int ordenId, string producto);
    Task<decimal> ObtenerTotalPorOrdenAsync(int ordenId);
    Task EliminarPorOrdenIdAsync(int ordenId);
    Task<IEnumerable<OrdenDetalle>> ObtenerPorProductoAsync(string producto);
    Task<Dictionary<string, int>> ObtenerProductosMasVendidosAsync(int cantidad = 10);
}
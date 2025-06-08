using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.DTOs.Response;
using OrdenCompra.Core.Entidades;

namespace OrdenCompra.Core.Interfaces.Repositorios;
public interface IOrdenRepositorio : IBaseRepositorio<Orden>
{
    Task<Orden?> ObtenerConDetallesAsync(int id);
    Task<ListaOrdenesResponse> ObtenerConFiltrosAsync(FiltroOrdenesRequest filtros);
    Task<bool> ExisteOrdenParaClienteEnFechaAsync(string cliente, DateTime fecha, int? ordenIdExcluir = null);
    Task<IEnumerable<Orden>> ObtenerPorUsuarioAsync(int usuarioId);
    Task<IEnumerable<Orden>> ObtenerPorClienteAsync(string cliente);
    Task<IEnumerable<Orden>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<decimal> ObtenerTotalVentasAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null);
    Task<int> ObtenerTotalOrdenesAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null);
}
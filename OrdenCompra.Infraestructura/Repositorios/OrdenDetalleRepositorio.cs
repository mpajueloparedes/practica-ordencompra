using Microsoft.EntityFrameworkCore;
using OrdenCompra.Core.Entidades;
using OrdenCompra.Core.Interfaces.Repositorios;
using OrdenCompra.Infraestructura.Data;

namespace OrdenCompra.Infraestructura.Repositorios;

public class OrdenDetalleRepositorio : BaseRepositorio<OrdenDetalle>, IOrdenDetalleRepositorio
{
    public OrdenDetalleRepositorio(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<OrdenDetalle>> ObtenerPorOrdenIdAsync(int ordenId)
    {
        return await _dbSet
            .Include(d => d.Orden)
            .Where(d => d.OrdenId == ordenId)
            .ToListAsync();
    }

    public async Task<OrdenDetalle?> ObtenerPorOrdenYProductoAsync(int ordenId, string producto)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.OrdenId == ordenId && d.Producto == producto);
    }

    public async Task<decimal> ObtenerTotalPorOrdenAsync(int ordenId)
    {
        return await _dbSet
            .Where(d => d.OrdenId == ordenId)
            .SumAsync(d => d.Subtotal);
    }

    public async Task EliminarPorOrdenIdAsync(int ordenId)
    {
        var detalles = await _dbSet.Where(d => d.OrdenId == ordenId).ToListAsync();
        _dbSet.RemoveRange(detalles);
    }

    public async Task<IEnumerable<OrdenDetalle>> ObtenerPorProductoAsync(string producto)
    {
        return await _dbSet
            .Include(d => d.Orden)
            .Where(d => d.Producto.Contains(producto))
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> ObtenerProductosMasVendidosAsync(int cantidad = 10)
    {
        return await _dbSet
            .GroupBy(d => d.Producto)
            .Select(g => new { Producto = g.Key, TotalVendido = g.Sum(d => d.Cantidad) })
            .OrderByDescending(x => x.TotalVendido)
            .Take(cantidad)
            .ToDictionaryAsync(x => x.Producto, x => x.TotalVendido);
    }
}
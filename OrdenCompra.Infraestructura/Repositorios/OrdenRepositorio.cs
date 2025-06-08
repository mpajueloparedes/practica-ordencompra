using Microsoft.EntityFrameworkCore;
using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.DTOs.Response;
using OrdenCompra.Core.Entidades;
using OrdenCompra.Core.Interfaces.Repositorios;
using OrdenCompra.Infraestructura.Data;

namespace OrdenCompra.Infraestructura.Repositorios;

public class OrdenRepositorio : BaseRepositorio<Orden>, IOrdenRepositorio
{
    public OrdenRepositorio(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Orden?> ObtenerConDetallesAsync(int id)
    {
        return await _dbSet
            .Include(o => o.Detalles)
            .Include(o => o.Usuario)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<ListaOrdenesResponse> ObtenerConFiltrosAsync(FiltroOrdenesRequest filtros)
    {
        var query = _dbSet.Include(o => o.Usuario).AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrEmpty(filtros.Cliente))
        {
            query = query.Where(o => o.Cliente.Contains(filtros.Cliente));
        }

        if (filtros.FechaInicio.HasValue)
        {
            query = query.Where(o => o.FechaCreacion >= filtros.FechaInicio.Value);
        }

        if (filtros.FechaFin.HasValue)
        {
            query = query.Where(o => o.FechaCreacion <= filtros.FechaFin.Value);
        }

        // Contar total de registros
        var totalRegistros = await query.CountAsync();

        // Aplicar ordenamiento
        query = filtros.OrdenarPor.ToLower() switch
        {
            "id" => filtros.OrdenarDireccion.ToUpper() == "ASC"
                ? query.OrderBy(o => o.Id)
                : query.OrderByDescending(o => o.Id),
            "cliente" => filtros.OrdenarDireccion.ToUpper() == "ASC"
                ? query.OrderBy(o => o.Cliente)
                : query.OrderByDescending(o => o.Cliente),
            "total" => filtros.OrdenarDireccion.ToUpper() == "ASC"
                ? query.OrderBy(o => o.Total)
                : query.OrderByDescending(o => o.Total),
            _ => filtros.OrdenarDireccion.ToUpper() == "ASC"
                ? query.OrderBy(o => o.FechaCreacion)
                : query.OrderByDescending(o => o.FechaCreacion)
        };

        // Aplicar paginación
        var ordenes = await query
            .Skip((filtros.Pagina - 1) * filtros.TamanoPagina)
            .Take(filtros.TamanoPagina)
            .Select(o => new OrdenResponse
            {
                Id = o.Id,
                FechaCreacion = o.FechaCreacion,
                Cliente = o.Cliente,
                Total = o.Total,
                Usuario = o.Usuario.NombreUsuario
            })
            .ToListAsync();

        return new ListaOrdenesResponse
        {
            Ordenes = ordenes,
            TotalRegistros = totalRegistros,
            PaginaActual = filtros.Pagina,
            TamanoPagina = filtros.TamanoPagina
        };
    }

    public async Task<bool> ExisteOrdenParaClienteEnFechaAsync(string cliente, DateTime fecha, int? ordenIdExcluir = null)
    {
        var fechaInicio = fecha.Date;
        var fechaFin = fecha.Date.AddDays(1);

        var query = _dbSet.Where(o => o.Cliente == cliente &&
                                      o.FechaCreacion >= fechaInicio &&
                                      o.FechaCreacion < fechaFin);

        if (ordenIdExcluir.HasValue)
        {
            query = query.Where(o => o.Id != ordenIdExcluir.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Orden>> ObtenerPorUsuarioAsync(int usuarioId)
    {
        return await _dbSet
            .Include(o => o.Detalles)
            .Include(o => o.Usuario)
            .Where(o => o.UsuarioId == usuarioId)
            .OrderByDescending(o => o.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Orden>> ObtenerPorClienteAsync(string cliente)
    {
        return await _dbSet
            .Include(o => o.Detalles)
            .Include(o => o.Usuario)
            .Where(o => o.Cliente.Contains(cliente))
            .OrderByDescending(o => o.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Orden>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _dbSet
            .Include(o => o.Detalles)
            .Include(o => o.Usuario)
            .Where(o => o.FechaCreacion >= fechaInicio && o.FechaCreacion <= fechaFin)
            .OrderByDescending(o => o.FechaCreacion)
            .ToListAsync();
    }

    public async Task<decimal> ObtenerTotalVentasAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        var query = _dbSet.AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(o => o.FechaCreacion >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(o => o.FechaCreacion <= fechaFin.Value);

        return await query.SumAsync(o => o.Total);
    }

    public async Task<int> ObtenerTotalOrdenesAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        var query = _dbSet.AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(o => o.FechaCreacion >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(o => o.FechaCreacion <= fechaFin.Value);

        return await query.CountAsync();
    }
}

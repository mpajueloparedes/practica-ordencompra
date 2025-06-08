using Microsoft.EntityFrameworkCore;
using OrdenCompra.Core.Interfaces.Repositorios;
using OrdenCompra.Infraestructura.Data;
using System.Linq.Expressions;

namespace OrdenCompra.Infraestructura.Repositorios;

public class BaseRepositorio<T> : IBaseRepositorio<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepositorio(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<T?> ObtenerPorIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> ObtenerTodosAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado)
    {
        return await _dbSet.Where(predicado).ToListAsync();
    }

    public virtual async Task<T?> BuscarUnoAsync(Expression<Func<T, bool>> predicado)
    {
        return await _dbSet.Where(predicado).FirstOrDefaultAsync();
    }

    public virtual async Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado)
    {
        return await _dbSet.AnyAsync(predicado);
    }

    public virtual async Task<int> ContarAsync(Expression<Func<T, bool>>? predicado = null)
    {
        if (predicado == null)
            return await _dbSet.CountAsync();

        return await _dbSet.CountAsync(predicado);
    }

    public virtual async Task AgregarAsync(T entidad)
    {
        await _dbSet.AddAsync(entidad);
    }

    public virtual void Actualizar(T entidad)
    {
        _dbSet.Update(entidad);
    }

    public virtual void Eliminar(T entidad)
    {
        _dbSet.Remove(entidad);
    }

    public virtual void EliminarRango(IEnumerable<T> entidades)
    {
        _dbSet.RemoveRange(entidades);
    }
}
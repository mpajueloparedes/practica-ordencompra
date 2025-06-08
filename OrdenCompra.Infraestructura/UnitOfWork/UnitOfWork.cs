using Microsoft.EntityFrameworkCore.Storage;
using OrdenCompra.Core.Interfaces;
using OrdenCompra.Core.Interfaces.Repositorios;
using OrdenCompra.Infraestructura.Data;
using OrdenCompra.Infraestructura.Repositorios;

namespace OrdenCompra.Infraestructura.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Ordenes = new OrdenRepositorio(_context);
        OrdenDetalles = new OrdenDetalleRepositorio(_context);
        Usuarios = new UsuarioRepositorio(_context);
    }

    public IOrdenRepositorio Ordenes { get; }
    public IOrdenDetalleRepositorio OrdenDetalles { get; }
    public IUsuarioRepositorio Usuarios { get; }

    public async Task<int> GuardarCambiosAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task IniciarTransaccionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task ConfirmarTransaccionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RevertirTransaccionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
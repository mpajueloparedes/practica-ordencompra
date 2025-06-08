using Microsoft.EntityFrameworkCore;
using OrdenCompra.Core.Entidades;
using OrdenCompra.Core.Interfaces.Repositorios;
using OrdenCompra.Infraestructura.Data;

namespace OrdenCompra.Infraestructura.Repositorios;

public class UsuarioRepositorio : BaseRepositorio<Usuario>, IUsuarioRepositorio
{
    public UsuarioRepositorio(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario)
    {
        return await _dbSet.AnyAsync(u => u.NombreUsuario == nombreUsuario);
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<Usuario>> ObtenerPorRolAsync(string rol)
    {
        return await _dbSet
            .Where(u => u.Rol == rol)
            .ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> ObtenerActivosAsync()
    {
        return await _dbSet
            .Where(u => u.EstaActivo)
            .ToListAsync();
    }

    public async Task DesactivarUsuarioAsync(int usuarioId)
    {
        var usuario = await _dbSet.FindAsync(usuarioId);
        if (usuario != null)
        {
            usuario.EstaActivo = false;
            _dbSet.Update(usuario);
        }
    }

    public async Task ActivarUsuarioAsync(int usuarioId)
    {
        var usuario = await _dbSet.FindAsync(usuarioId);
        if (usuario != null)
        {
            usuario.EstaActivo = true;
            _dbSet.Update(usuario);
        }
    }
}
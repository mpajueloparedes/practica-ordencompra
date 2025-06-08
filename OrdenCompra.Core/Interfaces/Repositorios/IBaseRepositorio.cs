using System.Linq.Expressions;
namespace OrdenCompra.Core.Interfaces.Repositorios;
public interface IBaseRepositorio<T> where T : class
{
    Task<T?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<T>> ObtenerTodosAsync();
    Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado);
    Task<T?> BuscarUnoAsync(Expression<Func<T, bool>> predicado);
    Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado);
    Task<int> ContarAsync(Expression<Func<T, bool>>? predicado = null);
    Task AgregarAsync(T entidad);
    void Actualizar(T entidad);
    void Eliminar(T entidad);
    void EliminarRango(IEnumerable<T> entidades);
}
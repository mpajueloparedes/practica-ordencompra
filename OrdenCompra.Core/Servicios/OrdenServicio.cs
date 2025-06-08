using OrdenCompra.Core.Constantes;
using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.DTOs.Response;
using OrdenCompra.Core.Entidades;
using OrdenCompra.Core.Excepciones;
using OrdenCompra.Core.Interfaces;
using OrdenCompra.Core.Interfaces.Servicios;

namespace OrdenCompra.Core.Servicios;

public class OrdenServicio : IOrdenServicio
{
    private readonly IUnitOfWork _unitOfWork;

    public OrdenServicio(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<OrdenResponse> CrearOrdenAsync(CrearOrdenRequest request, int usuarioId)
    {
        // Validar que el usuario existe
        var usuario = await _unitOfWork.Usuarios.ObtenerPorIdAsync(usuarioId);
        if (usuario == null)
            throw new RecursoNoEncontradoExcepcion(MensajesError.UsuarioNoEncontrado);

        // Validar regla de negocio: no órdenes sin detalles
        if (request.Detalles == null || !request.Detalles.Any())
            throw new ReglaDeNegocioExcepcion(MensajesError.OrdenSinDetalles);

        // Validar regla de negocio: no duplicados (mismo cliente y fecha)
        var fechaHoy = DateTime.Today;
        var existeOrden = await _unitOfWork.Ordenes.ExisteOrdenParaClienteEnFechaAsync(
            request.Cliente, fechaHoy);

        if (existeOrden)
            throw new ReglaDeNegocioExcepcion(MensajesError.OrdenDuplicada);

        try
        {
            await _unitOfWork.IniciarTransaccionAsync();

            // Crear la orden
            var orden = new Orden
            {
                Cliente = request.Cliente,
                UsuarioId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            // Crear los detalles
            var detalles = new List<OrdenDetalle>();
            foreach (var detalleRequest in request.Detalles)
            {
                var detalle = new OrdenDetalle
                {
                    Producto = detalleRequest.Producto,
                    Cantidad = detalleRequest.Cantidad,
                    PrecioUnitario = detalleRequest.PrecioUnitario
                };
                detalle.CalcularSubtotal();
                detalles.Add(detalle);
            }

            orden.Detalles = detalles;
            orden.CalcularTotal();

            await _unitOfWork.Ordenes.AgregarAsync(orden);
            await _unitOfWork.GuardarCambiosAsync();
            await _unitOfWork.ConfirmarTransaccionAsync();

            // Obtener la orden completa para la respuesta
            var ordenCompleta = await _unitOfWork.Ordenes.ObtenerConDetallesAsync(orden.Id);
            return MapearAOrdenResponse(ordenCompleta!);
        }
        catch
        {
            await _unitOfWork.RevertirTransaccionAsync();
            throw;
        }
    }

    public async Task<OrdenResponse> ActualizarOrdenAsync(ActualizarOrdenRequest request, int usuarioId)
    {
        var orden = await _unitOfWork.Ordenes.ObtenerConDetallesAsync(request.Id);
        if (orden == null)
            throw new RecursoNoEncontradoExcepcion(MensajesError.OrdenNoEncontrada);

        // Verificar permisos
        if (!await PuedeModificarOrdenAsync(request.Id, usuarioId))
            throw new ReglaDeNegocioExcepcion(MensajesError.UsuarioNoAutorizado);

        // Validar regla de negocio: no órdenes sin detalles
        if (request.Detalles == null || !request.Detalles.Any())
            throw new ReglaDeNegocioExcepcion(MensajesError.OrdenSinDetalles);

        // Validar regla de negocio: no duplicados (excluyendo la orden actual)
        var fechaOrden = orden.FechaCreacion.Date;
        var existeOrden = await _unitOfWork.Ordenes.ExisteOrdenParaClienteEnFechaAsync(
            request.Cliente, fechaOrden, request.Id);

        if (existeOrden)
            throw new ReglaDeNegocioExcepcion(MensajesError.OrdenDuplicada);

        try
        {
            await _unitOfWork.IniciarTransaccionAsync();

            // Actualizar datos de la orden
            orden.Cliente = request.Cliente;

            // Eliminar detalles existentes
            await _unitOfWork.OrdenDetalles.EliminarPorOrdenIdAsync(orden.Id);

            // Agregar nuevos detalles
            var nuevosDetalles = new List<OrdenDetalle>();
            foreach (var detalleRequest in request.Detalles)
            {
                var detalle = new OrdenDetalle
                {
                    OrdenId = orden.Id,
                    Producto = detalleRequest.Producto,
                    Cantidad = detalleRequest.Cantidad,
                    PrecioUnitario = detalleRequest.PrecioUnitario
                };
                detalle.CalcularSubtotal();
                nuevosDetalles.Add(detalle);
                await _unitOfWork.OrdenDetalles.AgregarAsync(detalle);
            }

            orden.Detalles = nuevosDetalles;
            orden.CalcularTotal();

            _unitOfWork.Ordenes.Actualizar(orden);
            await _unitOfWork.GuardarCambiosAsync();
            await _unitOfWork.ConfirmarTransaccionAsync();

            // Obtener la orden actualizada
            var ordenActualizada = await _unitOfWork.Ordenes.ObtenerConDetallesAsync(orden.Id);
            return MapearAOrdenResponse(ordenActualizada!);
        }
        catch
        {
            await _unitOfWork.RevertirTransaccionAsync();
            throw;
        }
    }

    public async Task EliminarOrdenAsync(int ordenId, int usuarioId)
    {
        var orden = await _unitOfWork.Ordenes.ObtenerPorIdAsync(ordenId);
        if (orden == null)
            throw new RecursoNoEncontradoExcepcion(MensajesError.OrdenNoEncontrada);

        // Verificar permisos
        if (!await PuedeEliminarOrdenAsync(ordenId, usuarioId))
            throw new ReglaDeNegocioExcepcion(MensajesError.UsuarioNoAutorizado);

        try
        {
            await _unitOfWork.IniciarTransaccionAsync();

            _unitOfWork.Ordenes.Eliminar(orden);
            await _unitOfWork.GuardarCambiosAsync();
            await _unitOfWork.ConfirmarTransaccionAsync();
        }
        catch
        {
            await _unitOfWork.RevertirTransaccionAsync();
            throw;
        }
    }

    public async Task<OrdenResponse?> ObtenerOrdenPorIdAsync(int ordenId)
    {
        var orden = await _unitOfWork.Ordenes.ObtenerConDetallesAsync(ordenId);
        return orden != null ? MapearAOrdenResponse(orden) : null;
    }

    public async Task<ListaOrdenesResponse> ObtenerOrdenesAsync(FiltroOrdenesRequest filtros)
    {
        if (!filtros.EsOrdenamientoValido())
        {
            filtros.OrdenarPor = "FechaCreacion";
            filtros.OrdenarDireccion = "DESC";
        }

        return await _unitOfWork.Ordenes.ObtenerConFiltrosAsync(filtros);
    }

    public async Task<IEnumerable<OrdenResponse>> ObtenerOrdenesPorUsuarioAsync(int usuarioId)
    {
        var ordenes = await _unitOfWork.Ordenes.ObtenerPorUsuarioAsync(usuarioId);
        return ordenes.Select(MapearAOrdenResponse);
    }

    public async Task<bool> PuedeEliminarOrdenAsync(int ordenId, int usuarioId)
    {
        var orden = await _unitOfWork.Ordenes.ObtenerPorIdAsync(ordenId);
        if (orden == null) return false;

        var usuario = await _unitOfWork.Usuarios.ObtenerPorIdAsync(usuarioId);
        if (usuario == null) return false;

        // Los administradores pueden eliminar cualquier orden
        if (usuario.Rol == "Administrador") return true;

        // Los usuarios solo pueden eliminar sus propias órdenes
        return orden.UsuarioId == usuarioId;
    }

    public async Task<bool> PuedeModificarOrdenAsync(int ordenId, int usuarioId)
    {
        var orden = await _unitOfWork.Ordenes.ObtenerPorIdAsync(ordenId);
        if (orden == null) return false;

        var usuario = await _unitOfWork.Usuarios.ObtenerPorIdAsync(usuarioId);
        if (usuario == null) return false;

        // Los administradores pueden modificar cualquier orden
        if (usuario.Rol == "Administrador") return true;

        // Los usuarios solo pueden modificar sus propias órdenes
        return orden.UsuarioId == usuarioId;
    }

    private static OrdenResponse MapearAOrdenResponse(Orden orden)
    {
        return new OrdenResponse
        {
            Id = orden.Id,
            FechaCreacion = orden.FechaCreacion,
            Cliente = orden.Cliente,
            Total = orden.Total,
            Usuario = orden.Usuario?.NombreUsuario ?? "N/A",
            Detalles = orden.Detalles?.Select(d => new OrdenDetalleResponse
            {
                Id = d.Id,
                OrdenId = d.OrdenId,
                Producto = d.Producto,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal
            }).ToList() ?? new List<OrdenDetalleResponse>()
        };
    }
}
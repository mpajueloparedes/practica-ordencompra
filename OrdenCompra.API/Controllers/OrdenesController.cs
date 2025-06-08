using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdenCompra.Core.DTOs.Auth;
using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.DTOs.Response;
using OrdenCompra.Core.Excepciones;
using OrdenCompra.Core.Interfaces.Servicios;
using System.Security.Claims;

namespace OrdenCompra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class OrdenesController : ControllerBase
{
    private readonly IOrdenServicio _ordenServicio;
    private readonly ILogger<OrdenesController> _logger;

    public OrdenesController(IOrdenServicio ordenServicio, ILogger<OrdenesController> logger)
    {
        _ordenServicio = ordenServicio ?? throw new ArgumentNullException(nameof(ordenServicio));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene una lista paginada de órdenes con filtros opcionales
    /// </summary>
    /// <param name="filtros">Filtros de búsqueda y paginación</param>
    /// <returns>Lista paginada de órdenes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ListaOrdenesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ListaOrdenesResponse>> ObtenerOrdenes([FromQuery] FiltroOrdenesRequest filtros)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _ordenServicio.ObtenerOrdenesAsync(filtros);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de órdenes");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una orden específica por su ID
    /// </summary>
    /// <param name="id">ID de la orden</param>
    /// <returns>Datos de la orden con sus detalles</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrdenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrdenResponse>> ObtenerOrdenPorId(int id)
    {
        try
        {
            var orden = await _ordenServicio.ObtenerOrdenPorIdAsync(id);

            if (orden == null)
            {
                return NotFound(new { mensaje = $"No se encontró la orden con ID {id}" });
            }

            return Ok(orden);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la orden con ID: {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea una nueva orden de compra
    /// </summary>
    /// <param name="request">Datos de la nueva orden</param>
    /// <returns>Orden creada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrdenResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrdenResponse>> CrearOrden([FromBody] CrearOrdenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioId = ObtenerUsuarioIdDelToken();
            var orden = await _ordenServicio.CrearOrdenAsync(request, usuarioId);

            _logger.LogInformation("Orden creada exitosamente con ID: {Id} por usuario: {UsuarioId}",
                orden.Id, usuarioId);

            return CreatedAtAction(nameof(ObtenerOrdenPorId), new { id = orden.Id }, orden);
        }
        catch (ReglaDeNegocioExcepcion ex)
        {
            _logger.LogWarning("Error de regla de negocio al crear orden: {Mensaje}", ex.Message);
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (ValidacionExcepcion ex)
        {
            _logger.LogWarning("Error de validación al crear orden: {Errores}", ex.Errores);
            return BadRequest(new { mensaje = "Errores de validación", errores = ex.Errores });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la orden");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza una orden existente
    /// </summary>
    /// <param name="id">ID de la orden a actualizar</param>
    /// <param name="request">Nuevos datos de la orden</param>
    /// <returns>Orden actualizada</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OrdenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OrdenResponse>> ActualizarOrden(int id, [FromBody] ActualizarOrdenRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del cuerpo de la petición" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioId = ObtenerUsuarioIdDelToken();
            var orden = await _ordenServicio.ActualizarOrdenAsync(request, usuarioId);

            _logger.LogInformation("Orden actualizada exitosamente con ID: {Id} por usuario: {UsuarioId}",
                orden.Id, usuarioId);

            return Ok(orden);
        }
        catch (RecursoNoEncontradoExcepcion ex)
        {
            _logger.LogWarning("Orden no encontrada para actualizar: {Mensaje}", ex.Message);
            return NotFound(new { mensaje = ex.Message });
        }
        catch (ReglaDeNegocioExcepcion ex)
        {
            _logger.LogWarning("Error de regla de negocio al actualizar orden: {Mensaje}", ex.Message);
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (ValidacionExcepcion ex)
        {
            _logger.LogWarning("Error de validación al actualizar orden: {Errores}", ex.Errores);
            return BadRequest(new { mensaje = "Errores de validación", errores = ex.Errores });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la orden con ID: {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina una orden existente
    /// </summary>
    /// <param name="id">ID de la orden a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> EliminarOrden(int id)
    {
        try
        {
            var usuarioId = ObtenerUsuarioIdDelToken();
            await _ordenServicio.EliminarOrdenAsync(id, usuarioId);

            _logger.LogInformation("Orden eliminada exitosamente con ID: {Id} por usuario: {UsuarioId}",
                id, usuarioId);

            return NoContent();
        }
        catch (RecursoNoEncontradoExcepcion ex)
        {
            _logger.LogWarning("Orden no encontrada para eliminar: {Mensaje}", ex.Message);
            return NotFound(new { mensaje = ex.Message });
        }
        catch (ReglaDeNegocioExcepcion ex)
        {
            _logger.LogWarning("Error de regla de negocio al eliminar orden: {Mensaje}", ex.Message);
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la orden con ID: {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene las órdenes del usuario autenticado
    /// </summary>
    /// <returns>Lista de órdenes del usuario</returns>
    [HttpGet("mis-ordenes")]
    [ProducesResponseType(typeof(IEnumerable<OrdenResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrdenResponse>>> ObtenerMisOrdenes()
    {
        try
        {
            var usuarioId = ObtenerUsuarioIdDelToken();
            var ordenes = await _ordenServicio.ObtenerOrdenesPorUsuarioAsync(usuarioId);
            return Ok(ordenes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las órdenes del usuario");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    private int ObtenerUsuarioIdDelToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int usuarioId))
        {
            throw new UnauthorizedAccessException("Token de usuario inválido");
        }

        return usuarioId;
    }
}
using Microsoft.AspNetCore.Mvc;
using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Web.Models.ViewModels;
using OrdenCompra.Web.Services;

namespace OrdenCompra.Web.Controllers;

public class OrdenesController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<OrdenesController> _logger;

    public OrdenesController(IApiService apiService, ILogger<OrdenesController> logger)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> Index(FiltroOrdenesViewModel filtros)
    {
        if (!EstaAutenticado())
            return RedirectToAction("Login", "Auth");

        try
        {
            // Validar y establecer valores por defecto
            if (string.IsNullOrEmpty(filtros.OrdenarPor))
                filtros.OrdenarPor = "FechaCreacion";

            if (string.IsNullOrEmpty(filtros.OrdenarDireccion))
                filtros.OrdenarDireccion = "DESC";

            if (filtros.Pagina < 1)
                filtros.Pagina = 1;

            if (filtros.TamanoPagina < 1 || filtros.TamanoPagina > 100)
                filtros.TamanoPagina = 10;

            // Validar campos de ordenamiento
            var camposValidos = new[] { "Id", "FechaCreacion", "Cliente", "Total" };
            if (!camposValidos.Contains(filtros.OrdenarPor))
                filtros.OrdenarPor = "FechaCreacion";

            var direccionesValidas = new[] { "ASC", "DESC" };
            if (!direccionesValidas.Contains(filtros.OrdenarDireccion.ToUpper()))
                filtros.OrdenarDireccion = "DESC";

            // Mapear a FiltroOrdenesRequest para la API
            var filtrosRequest = new FiltroOrdenesRequest
            {
                Pagina = filtros.Pagina,
                TamanoPagina = filtros.TamanoPagina,
                Cliente = filtros.Cliente,
                FechaInicio = filtros.FechaInicio,
                FechaFin = filtros.FechaFin,
                OrdenarPor = filtros.OrdenarPor,
                OrdenarDireccion = filtros.OrdenarDireccion.ToUpper()
            };

            var resultado = await _apiService.ObtenerOrdenesAsync(filtrosRequest);

            if (resultado != null)
            {
                var viewModel = new ListaOrdenesViewModel
                {
                    Ordenes = resultado.Ordenes.Select(MapearAOrdenViewModel).ToList(),
                    TotalRegistros = resultado.TotalRegistros,
                    PaginaActual = resultado.PaginaActual,
                    TamanoPagina = resultado.TamanoPagina,
                    Filtros = filtros
                };

                return View(viewModel);
            }

            return View(new ListaOrdenesViewModel { Filtros = filtros });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de órdenes");
            TempData["Error"] = "Error al cargar las órdenes. Por favor, intente más tarde.";
            return View(new ListaOrdenesViewModel { Filtros = filtros });
        }
    }

    public async Task<IActionResult> Detalles(int id)
    {
        if (!EstaAutenticado())
            return RedirectToAction("Login", "Auth");

        try
        {
            var orden = await _apiService.ObtenerOrdenPorIdAsync(id);

            if (orden == null)
            {
                TempData["Error"] = "Orden no encontrada.";
                return RedirectToAction("Index");
            }

            return View(MapearAOrdenViewModel(orden));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalles de la orden {Id}", id);
            TempData["Error"] = "Error al cargar los detalles de la orden.";
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public IActionResult Crear()
    {
        if (!EstaAutenticado())
            return RedirectToAction("Login", "Auth");

        var viewModel = new OrdenViewModel
        {
            Detalles = new List<OrdenDetalleViewModel>
            {
                new OrdenDetalleViewModel() // Agregar un detalle vacío para empezar
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(OrdenViewModel model)
    {
        if (!EstaAutenticado())
            return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new CrearOrdenRequest
            {
                Cliente = model.Cliente,
                Detalles = model.Detalles.Select(d => new CrearOrdenDetalleRequest
                {
                    Producto = d.Producto,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            var resultado = await _apiService.CrearOrdenAsync(request);

            if (resultado != null)
            {
                TempData["Success"] = "Orden creada exitosamente.";
                return RedirectToAction("Detalles", new { id = resultado.Id });
            }

            ModelState.AddModelError(string.Empty, "Error al crear la orden.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear orden");
            ModelState.AddModelError(string.Empty, "Error al crear la orden. " + ex.Message);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        if (!EstaAutenticado())
            return RedirectToAction("Login", "Auth");

        try
        {
            var orden = await _apiService.ObtenerOrdenPorIdAsync(id);

            if (orden == null)
            {
                TempData["Error"] = "Orden no encontrada.";
                return RedirectToAction("Index");
            }

            return View(MapearAOrdenViewModel(orden));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener orden para editar {Id}", id);
            TempData["Error"] = "Error al cargar la orden para editar.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(OrdenViewModel model)
    {
        if (!EstaAutenticado())
            return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new ActualizarOrdenRequest
            {
                Id = model.Id,
                Cliente = model.Cliente,
                Detalles = model.Detalles.Select(d => new ActualizarOrdenDetalleRequest
                {
                    Id = d.Id,
                    Producto = d.Producto,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            var resultado = await _apiService.ActualizarOrdenAsync(request);

            if (resultado != null)
            {
                TempData["Success"] = "Orden actualizada exitosamente.";
                return RedirectToAction("Detalles", new { id = resultado.Id });
            }

            ModelState.AddModelError(string.Empty, "Error al actualizar la orden.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar orden {Id}", model.Id);
            ModelState.AddModelError(string.Empty, "Error interno al actualizar la orden.");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!EstaAutenticado())
            return RedirectToAction("Login", "Auth");

        try
        {
            var orden = await _apiService.ObtenerOrdenPorIdAsync(id);

            if (orden == null)
            {
                TempData["Error"] = "Orden no encontrada.";
                return RedirectToAction("Index");
            }

            return View(MapearAOrdenViewModel(orden));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener orden para eliminar {Id}", id);
            TempData["Error"] = "Error al cargar la orden para eliminar.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarEliminacion(int id)
    {
        if (!EstaAutenticado())
            return RedirectToAction("Login", "Auth");

        try
        {
            var resultado = await _apiService.EliminarOrdenAsync(id);

            if (resultado)
            {
                TempData["Success"] = "Orden eliminada exitosamente.";
            }
            else
            {
                TempData["Error"] = "Error al eliminar la orden.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar orden {Id}", id);
            TempData["Error"] = "Error interno al eliminar la orden.";
        }

        return RedirectToAction("Index");
    }

    private bool EstaAutenticado()
    {
        var token = HttpContext.Session.GetString("Token");
        if (!string.IsNullOrEmpty(token))
        {
            _apiService.EstablecerToken(token);
            return true;
        }
        return false;
    }

    private static OrdenViewModel MapearAOrdenViewModel(Core.DTOs.Response.OrdenResponse orden)
    {
        return new OrdenViewModel
        {
            Id = orden.Id,
            FechaCreacion = orden.FechaCreacion,
            Cliente = orden.Cliente,
            Total = orden.Total,
            Usuario = orden.Usuario,
            Detalles = orden.Detalles.Select(d => new OrdenDetalleViewModel
            {
                Id = d.Id,
                OrdenId = d.OrdenId,
                Producto = d.Producto,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal
            }).ToList()
        };
    }
}
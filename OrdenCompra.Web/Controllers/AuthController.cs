using Microsoft.AspNetCore.Mvc;
using OrdenCompra.Core.DTOs.Auth;
using OrdenCompra.Web.Models.ViewModels;
using OrdenCompra.Web.Services;

namespace OrdenCompra.Web.Controllers;

public class AuthController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IApiService apiService, ILogger<AuthController> logger)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetString("Token") != null)
        {
            return RedirectToAction("Index", "Ordenes");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var loginRequest = new LoginRequest
            {
                NombreUsuario = model.NombreUsuario,
                Password = model.Password
            };

            var resultado = await _apiService.LoginAsync(loginRequest);

            if (resultado != null && resultado.EsExitoso)
            {
                // Guardar token en sesión
                HttpContext.Session.SetString("Token", resultado.Token);
                HttpContext.Session.SetString("NombreUsuario", resultado.NombreUsuario);
                HttpContext.Session.SetString("Email", resultado.Email);
                HttpContext.Session.SetString("Rol", resultado.Rol);

                // Establecer token en el servicio API
                _apiService.EstablecerToken(resultado.Token);

                _logger.LogInformation("Usuario {Usuario} inició sesión exitosamente", model.NombreUsuario);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Ordenes");
            }
            else
            {
                ModelState.AddModelError(string.Empty, resultado?.MensajeError ?? "Credenciales inválidas");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el login");
            ModelState.AddModelError(string.Empty, "Error interno. Por favor, intente más tarde.");
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        _apiService.LimpiarToken();
        _logger.LogInformation("Usuario cerró sesión");
        return RedirectToAction("Index", "Home");
    }
}
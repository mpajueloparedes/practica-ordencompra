using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdenCompra.Core.DTOs.Auth;
using OrdenCompra.Core.Interfaces.Servicios;

namespace OrdenCompra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthServicio _authServicio;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthServicio authServicio, ILogger<AuthController> logger)
    {
        _authServicio = authServicio ?? throw new ArgumentNullException(nameof(authServicio));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Autentica un usuario y devuelve un token JWT
    /// </summary>
    /// <param name="request">Credenciales de login</param>
    /// <returns>Token JWT y información del usuario</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _authServicio.LoginAsync(request);

            if (!resultado.EsExitoso)
            {
                _logger.LogWarning("Intento de login fallido para usuario: {Usuario}", request.NombreUsuario);
                return Unauthorized(new { mensaje = resultado.MensajeError });
            }

            _logger.LogInformation("Login exitoso para usuario: {Usuario}", request.NombreUsuario);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el login del usuario: {Usuario}", request.NombreUsuario);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Valida un token JWT
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>Resultado de la validación</returns>
    [HttpPost("validar-token")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ValidarToken([FromBody] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { mensaje = "Token requerido" });
            }

            var esValido = _authServicio.ValidarToken(token);
            var usuario = await _authServicio.ObtenerUsuarioPorTokenAsync(token);

            return Ok(new
            {
                esValido,
                usuario = esValido && usuario != null ? new
                {
                    usuario.Id,
                    usuario.NombreUsuario,
                    usuario.Email,
                    usuario.Rol
                } : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar token");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }


    [HttpPost("generar-hash")]
    public async Task<IActionResult> GenerarHash([FromBody] string password)
    {
        var hash = await _authServicio.HashPasswordAsync(password);
        return Ok(new { password, hash });
    }

    [HttpGet("test-auth")]
    [Authorize]
    public IActionResult TestAuth()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(new
        {
            isAuthenticated = User.Identity?.IsAuthenticated,
            claims = claims,
            name = User.Identity?.Name
        });
    }

    [HttpGet("debug-auth")]
    [AllowAnonymous] // Sin autorización para poder diagnosticar
    public IActionResult DebugAuth()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        var hasBearer = authHeader?.StartsWith("Bearer ") == true;
        var token = hasBearer ? authHeader?.Substring("Bearer ".Length).Trim() : null;

        return Ok(new
        {
            hasAuthHeader = !string.IsNullOrEmpty(authHeader),
            authHeader = authHeader,
            hasBearer = hasBearer,
            tokenLength = token?.Length ?? 0,
            isAuthenticated = User.Identity?.IsAuthenticated,
            claimsCount = User.Claims.Count(),
            claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }



}
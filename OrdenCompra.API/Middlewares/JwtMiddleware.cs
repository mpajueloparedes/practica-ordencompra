using Microsoft.IdentityModel.Tokens;
using OrdenCompra.Core.Interfaces.Servicios;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrdenCompra.API.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuthServicio authServicio)
    {
        var token = ExtractTokenFromHeader(context);

        if (!string.IsNullOrEmpty(token))
        {
            await AttachUserToContext(context, authServicio, token);
        }

        await _next(context);
    }

    private static string? ExtractTokenFromHeader(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        return null;
    }

    private async Task AttachUserToContext(HttpContext context, IAuthServicio authServicio, string token)
    {
        try
        {
            var usuario = await authServicio.ObtenerUsuarioPorTokenAsync(token);

            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new(ClaimTypes.Name, usuario.NombreUsuario),
                    new(ClaimTypes.Email, usuario.Email),
                    new(ClaimTypes.Role, usuario.Rol)
                };

                var identity = new ClaimsIdentity(claims, "jwt");
                context.User = new ClaimsPrincipal(identity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al procesar token JWT");
        }
    }
}

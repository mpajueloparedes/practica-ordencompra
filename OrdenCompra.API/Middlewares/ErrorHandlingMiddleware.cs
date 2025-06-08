using OrdenCompra.Core.Constantes;
using OrdenCompra.Core.Excepciones;
using System.Net;
using System.Text.Json;

namespace OrdenCompra.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no manejada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Definir la estructura base del response
        object response;

        switch (exception)
        {
            case RecursoNoEncontradoExcepcion:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    mensaje = exception.Message,
                    detalles = (string?)null,
                    timestamp = DateTime.UtcNow
                };
                break;

            case ReglaDeNegocioExcepcion:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    mensaje = exception.Message,
                    detalles = (string?)null,
                    timestamp = DateTime.UtcNow
                };
                break;

            case ValidacionExcepcion validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    mensaje = "Errores de validación",
                    detalles = JsonSerializer.Serialize(validationEx.Errores),
                    timestamp = DateTime.UtcNow
                };
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    mensaje = "No autorizado",
                    detalles = (string?)null,
                    timestamp = DateTime.UtcNow
                };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    mensaje = "Error interno del servidor",
                    detalles = exception.Message,
                    timestamp = DateTime.UtcNow
                };
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

}
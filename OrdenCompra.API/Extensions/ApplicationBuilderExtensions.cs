using OrdenCompra.API.Middlewares;
using OrdenCompra.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace OrdenCompra.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsarMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        return app;
    }

    public static async Task<IApplicationBuilder> InicializarBaseDeDatos(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error al inicializar la base de datos");
        }

        return app;
    }
}
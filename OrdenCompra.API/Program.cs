using OrdenCompra.API.Extensions;
using OrdenCompra.API.Filters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Agregar servicios al contenedor
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidacionModeloFilter>();
});

builder.Services.AddEndpointsApiExplorer();

// Servicios personalizados
builder.Services.AgregarServicios(builder.Configuration);
builder.Services.AgregarAutenticacion(builder.Configuration);
builder.Services.AgregarSwagger();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Órdenes de Compra v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
//app.UsarMiddlewares();
app.UseMiddleware<OrdenCompra.API.Middlewares.ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Inicializar base de datos
await app.InicializarBaseDeDatos();

try
{
    Log.Information("Iniciando aplicación");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error fatal al iniciar la aplicación");
}
finally
{
    Log.CloseAndFlush();
}
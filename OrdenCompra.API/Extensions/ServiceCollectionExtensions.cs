using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrdenCompra.API.Filters;
using OrdenCompra.Core.Interfaces;
using OrdenCompra.Core.Interfaces.Repositorios;
using OrdenCompra.Core.Interfaces.Servicios;
using OrdenCompra.Core.Servicios;
using OrdenCompra.Core.Validadores;
using OrdenCompra.Infraestructura.Data;
using OrdenCompra.Infraestructura.Repositorios;
using OrdenCompra.Infraestructura.UnitOfWork;
using System.Reflection;
using System.Text;

namespace OrdenCompra.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AgregarServicios(this IServiceCollection services, IConfiguration configuration)
    {
        // Base de datos
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositorios
        services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();
        services.AddScoped<IOrdenDetalleRepositorio, OrdenDetalleRepositorio>();
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Servicios
        services.AddScoped<IOrdenServicio, OrdenServicio>();
        services.AddScoped<IAuthServicio>(provider =>
        {
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var jwtSecret = configuration["Jwt:SecretKey"] ?? "wA7f!z9LxP2@cVn6RbTuE#0mJdYqKw4Z";
            var jwtIssuer = configuration["Jwt:Issuer"] ?? "OrdenCompraAPI";
            var jwtExpiration = int.Parse(configuration["Jwt:ExpirationHours"] ?? "24");

            return new AuthServicio(unitOfWork, jwtSecret, jwtIssuer, jwtExpiration);
        });

        // Validadores
        services.AddValidatorsFromAssemblyContaining<CrearOrdenValidator>();

        // Filtros
        services.AddScoped<ValidacionModeloFilter>();

        return services;
    }

    public static IServiceCollection AgregarAutenticacion(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:SecretKey"] ?? "wA7f!z9LxP2@cVn6RbTuE#0mJdYqKw4Z";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "OrdenCompraAPI";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtIssuer,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrador"));
            options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("Usuario", "Administrador"));
        });

        return services;
    }

    public static IServiceCollection AgregarSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API de Órdenes de Compra",
                Version = "v1",
                Description = "API para gestionar órdenes de compra con autenticación JWT",
                Contact = new OpenApiContact
                {
                    Name = "Equipo de Desarrollo",
                    Email = "desarrollo@ordencompra.com"
                }
            });

            // Configuración para JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Autorización JWT usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            // Incluir comentarios XML
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
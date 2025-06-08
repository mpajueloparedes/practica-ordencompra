using Microsoft.EntityFrameworkCore;
using OrdenCompra.Core.Entidades;
using OrdenCompra.Infraestructura.Data.Configuraciones;

namespace OrdenCompra.Infraestructura.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Orden> Ordenes { get; set; }
    public DbSet<OrdenDetalle> OrdenDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UsuarioConfiguracion());
        modelBuilder.ApplyConfiguration(new OrdenConfiguracion());
        modelBuilder.ApplyConfiguration(new OrdenDetalleConfiguracion());
    }
}
using System.ComponentModel.DataAnnotations;
namespace OrdenCompra.Core.Entidades;
public class Orden
{
    public int Id { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "El cliente es requerido")]
    [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder 100 caracteres")]
    public string Cliente { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "El total debe ser mayor o igual a cero")]
    public decimal Total { get; set; }

    [Required(ErrorMessage = "El usuario es requerido")]
    public int UsuarioId { get; set; }

    // Navegación
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<OrdenDetalle> Detalles { get; set; } = new List<OrdenDetalle>();

    // Métodos de negocio
    public void CalcularTotal()
    {
        Total = Detalles?.Sum(d => d.Subtotal) ?? 0;
    }

    public void AgregarDetalle(OrdenDetalle detalle)
    {
        if (detalle == null)
            throw new ArgumentNullException(nameof(detalle));

        detalle.OrdenId = Id;
        Detalles.Add(detalle);
        CalcularTotal();
    }

    public void RemoverDetalle(OrdenDetalle detalle)
    {
        if (detalle == null)
            throw new ArgumentNullException(nameof(detalle));

        Detalles.Remove(detalle);
        CalcularTotal();
    }

    public bool TieneDetalles()
    {
        return Detalles?.Any() == true;
    }
}
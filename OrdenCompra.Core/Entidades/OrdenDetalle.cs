using System.ComponentModel.DataAnnotations;
namespace OrdenCompra.Core.Entidades;
public class OrdenDetalle
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La orden es requerida")]
    public int OrdenId { get; set; }

    [Required(ErrorMessage = "El producto es requerido")]
    [StringLength(200, ErrorMessage = "El nombre del producto no puede exceder 200 caracteres")]
    public string Producto { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    public int Cantidad { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor o igual a cero")]
    public decimal PrecioUnitario { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor o igual a cero")]
    public decimal Subtotal { get; set; }

    // Navegación
    public virtual Orden Orden { get; set; } = null!;

    // Métodos de negocio
    public void CalcularSubtotal()
    {
        Subtotal = Cantidad * PrecioUnitario;
    }

    public void ActualizarCantidad(int nuevaCantidad)
    {
        if (nuevaCantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(nuevaCantidad));

        Cantidad = nuevaCantidad;
        CalcularSubtotal();
    }

    public void ActualizarPrecio(decimal nuevoPrecio)
    {
        if (nuevoPrecio < 0)
            throw new ArgumentException("El precio no puede ser negativo", nameof(nuevoPrecio));

        PrecioUnitario = nuevoPrecio;
        CalcularSubtotal();
    }
}
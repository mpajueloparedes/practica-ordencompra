using System.ComponentModel.DataAnnotations;

namespace OrdenCompra.Web.Models.ViewModels;

public class OrdenDetalleViewModel
{
    public int Id { get; set; }

    public int OrdenId { get; set; }

    [Required(ErrorMessage = "El producto es requerido")]
    [Display(Name = "Producto")]
    public string Producto { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    [Display(Name = "Cantidad")]
    public int Cantidad { get; set; }

    [Required(ErrorMessage = "El precio unitario es requerido")]
    [Range(0, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor o igual a cero")]
    [Display(Name = "Precio Unitario")]
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public decimal PrecioUnitario { get; set; }

    [Display(Name = "Subtotal")]
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public decimal Subtotal { get; set; }

    [Display(Name = "Precio Unitario")]
    public string PrecioUnitarioFormateado => PrecioUnitario.ToString("C");

    [Display(Name = "Subtotal")]
    public string SubtotalFormateado => Subtotal.ToString("C");
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdenCompra.Core.DTOs.Request;
public class ActualizarOrdenDetalleRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El producto es requerido")]
    [StringLength(100, ErrorMessage = "El nombre del producto no puede exceder 100 caracteres")]
    public string Producto { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    public int Cantidad { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor o igual a cero")]
    public decimal PrecioUnitario { get; set; }
}
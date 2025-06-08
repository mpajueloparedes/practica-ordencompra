using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdenCompra.Core.DTOs.Request;
public class ActualizarOrdenRequest
{
    [Required(ErrorMessage = "El ID es requerido")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El cliente es requerido")]
    [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder 100 caracteres")]
    public string Cliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los detalles son requeridos")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos un detalle")]
    public List<ActualizarOrdenDetalleRequest> Detalles { get; set; } = new();
}
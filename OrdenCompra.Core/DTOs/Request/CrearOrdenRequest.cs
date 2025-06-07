using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdenCompra.Core.DTOs.Request;
public class CrearOrdenRequest
{
    [Required(ErrorMessage = "El cliente es requerido")]
    [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder 100 caracteres")]
    public string Cliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los detalles son requeridos")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos un detalle")]
    public List<CrearOrdenDetalleRequest> Detalles { get; set; } = new();
}
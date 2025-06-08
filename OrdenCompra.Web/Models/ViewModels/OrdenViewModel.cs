using System.ComponentModel.DataAnnotations;

namespace OrdenCompra.Web.Models.ViewModels;

public class OrdenViewModel
{
    public int Id { get; set; }

    public DateTime FechaCreacion { get; set; }

    [Required(ErrorMessage = "El cliente es requerido")]
    [Display(Name = "Cliente")]
    public string Cliente { get; set; } = string.Empty;

    [Display(Name = "Total")]
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public decimal Total { get; set; }

    [Display(Name = "Usuario")]
    public string Usuario { get; set; } = string.Empty;

    [Display(Name = "Detalles")]
    public List<OrdenDetalleViewModel> Detalles { get; set; } = new();

    [Display(Name = "Fecha de Creación")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public string FechaCreacionFormateada => FechaCreacion.ToString("dd/MM/yyyy HH:mm");

    [Display(Name = "Total Formateado")]
    public string TotalFormateado => Total.ToString("C");

    [Display(Name = "Cantidad de Detalles")]
    public int TotalDetalles => Detalles?.Count ?? 0;
}

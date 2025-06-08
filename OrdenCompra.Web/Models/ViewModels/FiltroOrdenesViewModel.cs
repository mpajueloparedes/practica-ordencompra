using System.ComponentModel.DataAnnotations;

namespace OrdenCompra.Web.Models.ViewModels;

public class FiltroOrdenesViewModel
{
    [Display(Name = "Cliente")]
    public string? Cliente { get; set; }

    [Display(Name = "Fecha Inicio")]
    [DataType(DataType.Date)]
    public DateTime? FechaInicio { get; set; }

    [Display(Name = "Fecha Fin")]
    [DataType(DataType.Date)]
    public DateTime? FechaFin { get; set; }

    [Display(Name = "Ordenar Por")]
    public string OrdenarPor { get; set; } = "FechaCreacion";

    [Display(Name = "Dirección")]
    public string OrdenarDireccion { get; set; } = "DESC";

    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 10;
}

namespace OrdenCompra.Web.Models.ViewModels;

public class ListaOrdenesViewModel
{
    public List<OrdenViewModel> Ordenes { get; set; } = new();
    public int TotalRegistros { get; set; }
    public int PaginaActual { get; set; }
    public int TamanoPagina { get; set; }
    public FiltroOrdenesViewModel Filtros { get; set; } = new();

    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanoPagina);
    public bool TienePaginaAnterior => PaginaActual > 1;
    public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
}
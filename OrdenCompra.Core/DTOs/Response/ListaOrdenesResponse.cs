namespace OrdenCompra.Core.DTOs.Response;
public class ListaOrdenesResponse
{
    public List<OrdenResponse> Ordenes { get; set; } = new();
    public int TotalRegistros { get; set; }
    public int PaginaActual { get; set; }
    public int TamanoPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanoPagina);
    public bool TienePaginaAnterior => PaginaActual > 1;
    public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
}
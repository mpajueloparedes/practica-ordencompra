namespace OrdenCompra.Core.DTOs.Response;
public class OrdenResponse
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public List<OrdenDetalleResponse> Detalles { get; set; } = new();

    public string FechaCreacionFormateada => FechaCreacion.ToString("dd/MM/yyyy HH:mm");
    public string TotalFormateado => Total.ToString("C");
    public int TotalDetalles => Detalles?.Count ?? 0;
}
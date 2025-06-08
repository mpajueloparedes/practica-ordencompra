namespace OrdenCompra.Core.DTOs.Response;
public class OrdenDetalleResponse
{
    public int Id { get; set; }
    public int OrdenId { get; set; }
    public string Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public string PrecioUnitarioFormateado => PrecioUnitario.ToString("C");
    public string SubtotalFormateado => Subtotal.ToString("C");
}
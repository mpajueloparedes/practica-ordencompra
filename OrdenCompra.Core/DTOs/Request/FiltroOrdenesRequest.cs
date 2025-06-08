using System.ComponentModel.DataAnnotations;

namespace OrdenCompra.Core.DTOs.Request;
public class FiltroOrdenesRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor a cero")]
    public int Pagina { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
    public int TamanoPagina { get; set; } = 10;

    public string? Cliente { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string OrdenarPor { get; set; } = "FechaCreacion";

    public string OrdenarDireccion { get; set; } = "DESC";

    // Validación de ordenamiento
    public bool EsOrdenamientoValido()
    {
        var camposValidos = new[] { "Id", "FechaCreacion", "Cliente", "Total" };
        var direccionesValidas = new[] { "ASC", "DESC" };

        return camposValidos.Contains(OrdenarPor) &&
               direccionesValidas.Contains(OrdenarDireccion.ToUpper());
    }
}

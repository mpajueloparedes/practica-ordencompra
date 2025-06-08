namespace OrdenCompra.Core.DTOs.Auth;
public class LoginResponse
{
    public bool EsExitoso { get; set; }
    public string Token { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime FechaExpiracion { get; set; }

    public string? MensajeError { get; set; }
    public string? MensajeErrorDetalle { get; set; }
}
using System.ComponentModel.DataAnnotations;
namespace OrdenCompra.Core.DTOs.Auth;
public class LoginRequest
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    public string Password { get; set; } = string.Empty;
}
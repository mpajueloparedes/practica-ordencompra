using System.ComponentModel.DataAnnotations;

namespace OrdenCompra.Web.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [Display(Name = "Nombre de Usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

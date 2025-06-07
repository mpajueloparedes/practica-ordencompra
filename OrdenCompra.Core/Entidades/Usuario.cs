using System.ComponentModel.DataAnnotations;
namespace OrdenCompra.Core.Entidades:
   public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es requerido")]
    [StringLength(20, ErrorMessage = "El rol no puede exceder 20 caracteres")]
    public string Rol { get; set; } = "Usuario";

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public bool EstaActivo { get; set; } = true;

    // Navegación
    public virtual ICollection<Orden> Ordenes { get; set; } = new List<Orden>();
}
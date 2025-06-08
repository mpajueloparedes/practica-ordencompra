namespace OrdenCompra.Core.Constantes;
public static class MensajesError
{
    // Mensajes de Orden
    public const string OrdenNoEncontrada = "La orden especificada no fue encontrada.";
    public const string OrdenSinDetalles = "No se puede crear una orden sin detalles.";
    public const string OrdenDuplicada = "Ya existe una orden para este cliente en la fecha especificada.";
    public const string OrdenNoSePuedeEliminar = "La orden no se puede eliminar debido a restricciones de negocio.";

    // Mensajes de OrdenDetalle
    public const string DetalleNoEncontrado = "El detalle especificado no fue encontrado.";
    public const string CantidadInvalida = "La cantidad debe ser mayor a cero.";
    public const string PrecioInvalido = "El precio no puede ser negativo.";

    // Mensajes de Usuario
    public const string UsuarioNoEncontrado = "El usuario especificado no fue encontrado.";
    public const string CredencialesInvalidas = "Las credenciales proporcionadas son incorrectas.";
    public const string UsuarioNoAutorizado = "No tiene permisos para realizar esta acción.";
    public const string TokenInvalido = "El token proporcionado es inválido o ha expirado.";

    // Mensajes Generales
    public const string ErrorInterno = "Ha ocurrido un error interno. Por favor, intente más tarde.";
    public const string DatosInvalidos = "Los datos proporcionados no son válidos.";
    public const string OperacionNoPermitida = "La operación solicitada no está permitida.";
}
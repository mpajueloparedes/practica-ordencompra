namespace OrdenCompra.Core.Excepciones;
public class ReglaDeNegocioExcepcion : Exception
{
    public ReglaDeNegocioExcepcion(string mensaje) : base(mensaje) { }

    public ReglaDeNegocioExcepcion(string mensaje, Exception innerException)
        : base(mensaje, innerException) { }
}
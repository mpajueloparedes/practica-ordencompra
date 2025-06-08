namespace OrdenCompra.Core.Excepciones;
public class RecursoNoEncontradoExcepcion : Exception
{
    public RecursoNoEncontradoExcepcion(string mensaje) : base(mensaje) { }

    public RecursoNoEncontradoExcepcion(string recurso, object clave)
        : base($"El {recurso} con ID '{clave}' no fue encontrado.") { }
}
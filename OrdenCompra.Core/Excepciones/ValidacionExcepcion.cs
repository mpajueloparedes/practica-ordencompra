namespace OrdenCompra.Core.Excepciones;
public class ValidacionExcepcion : Exception
{
    public IDictionary<string, string[]> Errores { get; }

    public ValidacionExcepcion() : base("Se produjeron uno o más errores de validación.")
    {
        Errores = new Dictionary<string, string[]>();
    }

    public ValidacionExcepcion(IDictionary<string, string[]> errores) : this()
    {
        Errores = errores;
    }

    public ValidacionExcepcion(string campo, string error) : this()
    {
        Errores.Add(campo, new[] { error });
    }
}
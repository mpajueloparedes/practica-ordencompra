using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrdenCompra.API.Filters;

public class ValidacionModeloFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errores = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                );

            var response = new
            {
                mensaje = "Errores de validación",
                errores = errores,
                timestamp = DateTime.UtcNow
            };

            context.Result = new BadRequestObjectResult(response);
        }

        base.OnActionExecuting(context);
    }
}
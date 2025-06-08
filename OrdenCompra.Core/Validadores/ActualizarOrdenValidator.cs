using FluentValidation;
using OrdenCompra.Core.DTOs.Request;

namespace OrdenCompra.Core.Validadores;

public class ActualizarOrdenValidator : AbstractValidator<ActualizarOrdenRequest>
{
    public ActualizarOrdenValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID debe ser mayor a cero");

        RuleFor(x => x.Cliente)
            .NotEmpty().WithMessage("El cliente es requerido")
            .MaximumLength(100).WithMessage("El cliente no puede exceder 100 caracteres");

        RuleFor(x => x.Detalles)
            .NotEmpty().WithMessage("Debe incluir al menos un detalle")
            .Must(detalles => detalles != null && detalles.Any())
            .WithMessage("Los detalles son requeridos");

        RuleForEach(x => x.Detalles).SetValidator(new ActualizarOrdenDetalleValidator());
    }
}

public class ActualizarOrdenDetalleValidator : AbstractValidator<ActualizarOrdenDetalleRequest>
{
    public ActualizarOrdenDetalleValidator()
    {
        RuleFor(x => x.Producto)
            .NotEmpty().WithMessage("El producto es requerido")
            .MaximumLength(200).WithMessage("El producto no puede exceder 200 caracteres");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThanOrEqualTo(0).WithMessage("El precio unitario debe ser mayor o igual a cero");
    }
}
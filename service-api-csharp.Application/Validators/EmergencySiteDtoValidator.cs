using FluentValidation;
using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Validators;

public class EmergencySiteDtoValidator : AbstractValidator<EmergencySiteDto>
{
    public EmergencySiteDtoValidator()
    {
        RuleFor(x => x.NameSite)
            .NotEmpty().WithMessage("El nombre del sitio es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre del sitio no puede exceder los 200 caracteres.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .MaximumLength(200).WithMessage("El teléfono no puede exceder los 200 caracteres.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("La dirección de ubicación es obligatoria.")
            .MaximumLength(200).WithMessage("La dirección no puede exceder los 200 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La categoria es obligatoria.");
    }
}

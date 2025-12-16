using FluentValidation;
using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Validators;

public class UpdateReportDtoValidator : AbstractValidator<UpdateReportDto>
{
    public UpdateReportDtoValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty().WithMessage("El ID del reporte es obligatorio.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(200).WithMessage("El título no puede exceder los 200 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.");
    }
}

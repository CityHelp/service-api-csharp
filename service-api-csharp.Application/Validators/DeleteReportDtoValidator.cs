using FluentValidation;
using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Validators;

public class DeleteReportDtoValidator : AbstractValidator<DeleteReportDto>
{
    public DeleteReportDtoValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty().WithMessage("El ID del reporte es obligatorio.");
    }
}

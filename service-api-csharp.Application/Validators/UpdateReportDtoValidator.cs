using FluentValidation;
using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Validators;

public class UpdateReportDtoValidator : AbstractValidator<UpdateReportDto>
{
    public UpdateReportDtoValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty().WithMessage("Report ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }
}

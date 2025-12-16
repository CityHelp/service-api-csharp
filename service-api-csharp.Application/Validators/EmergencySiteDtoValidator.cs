using FluentValidation;
using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Validators;

public class EmergencySiteDtoValidator : AbstractValidator<EmergencySiteDto>
{
    public EmergencySiteDtoValidator()
    {
        RuleFor(x => x.NameSite)
            .NotEmpty().WithMessage("Site name is required.")
            .MaximumLength(200).WithMessage("Site name cannot exceed 200 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(200).WithMessage("Phone number cannot exceed 200 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Category is required.");
    }
}

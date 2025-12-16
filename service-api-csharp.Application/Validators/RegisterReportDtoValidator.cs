using FluentValidation;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Validators;

public class RegisterReportDtoValidator : AbstractValidator<RegisterReportDto>
{
    public RegisterReportDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.IdCategory)
            .NotEmpty().WithMessage("Category is required.")
            .Must(BeValidInt).WithMessage("Category ID must be a valid number.");

        RuleFor(x => x.EmergencyLevel)
            .NotEmpty().WithMessage("Emergency level is required.")
            .Matches("^(low|medium|high|critical)$").WithMessage("Emergency level must be: low, medium, high, or critical.");

        RuleFor(x => x.DateReport)
            .NotEmpty().WithMessage("Report date is required.");

        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage(Messages.Coordinates.LatitudeRequired)
            .Must(BeAValidDouble).WithMessage(Messages.Coordinates.LatitudeIsANumber)
            .Must(BeValidLatitude).WithMessage(Messages.Coordinates.LatitudeIsAValidNumber);

        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage(Messages.Coordinates.LongitudeRequired)
            .Must(BeAValidDouble).WithMessage(Messages.Coordinates.LongitudeIsANumber)
            .Must(BeValidLongitude).WithMessage(Messages.Coordinates.LongitudeIsAValidNumber);
    }

    private bool BeValidInt(string value)
    {
        return int.TryParse(value, out _);
    }

    private bool BeAValidDouble(string value)
    {
        return double.TryParse(value, out _);
    }

    private bool BeValidLatitude(string value)
    {
        if (!double.TryParse(value, out var number))
            return false;

        return number >= -90 && number <= 90;
    }

    private bool BeValidLongitude(string value)
    {
        if (!double.TryParse(value, out var number))
            return false;

        return number >= -180 && number <= 180;
    }
}

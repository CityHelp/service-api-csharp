using FluentValidation;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Validators;

public class ReportsRadio3kmDtoValidator : AbstractValidator<ReportsRadio3kmDto>
{
    public ReportsRadio3kmDtoValidator()
    {
        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage(Messages.Coordinates.LatitudeObligatory)
            .Must(BeAValidDouble).WithMessage(Messages.Coordinates.LatitudeBeANumber)
            .Must(BeValidLatitude).WithMessage(Messages.Coordinates.LatitudeBeAValidNumber);

        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage(Messages.Coordinates.LongitudeObligatory)
            .Must(BeAValidDouble).WithMessage(Messages.Coordinates.LongitudeBeANumber)
            .Must(BeValidLongitude).WithMessage(Messages.Coordinates.LongitudeBeAValidNumber);
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

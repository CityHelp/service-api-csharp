using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Validators;

using FluentValidation;

public class UbicationUserDtoValidator : AbstractValidator<UbicationUserDto>
{
    public UbicationUserDtoValidator()
    {
        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage(Messages.Coordinates.LatitudeRequired)
            .Must(BeAValidDouble).WithMessage(Messages.Coordinates.LatitudeIsANumber)
            .Must(BeValidLatitude).WithMessage(Messages.Coordinates.LatitudeIsAValidNumber);

        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage(Messages.Coordinates.LongitudeRequired)
            .Must(BeAValidDouble).WithMessage(Messages.Coordinates.LongitudeIsANumber)
            .Must(BeValidLongitude).WithMessage(Messages.Coordinates.LongitudeIsAValidNumber);
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

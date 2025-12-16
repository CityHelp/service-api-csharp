using FluentValidation;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Validators;

public class RegisterReportDtoValidator : AbstractValidator<RegisterReportDto>
{
    public RegisterReportDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(200).WithMessage("El título no puede exceder los 200 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.");

        RuleFor(x => x.IdCategory)
            .NotEmpty().WithMessage("La categoría es obligatoria.")
            .Must(BeValidInt).WithMessage("El ID de la categoría debe ser un número válido.");

        RuleFor(x => x.EmergencyLevel)
            .NotEmpty().WithMessage("El nivel de emergencia es obligatorio.")
            .Matches("^(baja|media|alta|critica)$").WithMessage("El nivel de emergencia debe ser: baja, media, alta o critica.");

        RuleFor(x => x.DateReport)
            .NotEmpty().WithMessage("La fecha del reporte es obligatoria.");

        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage(Messages.Coordinates.LatitudeObligatory)
            .Must(BeAValidDouble).WithMessage(Messages.Coordinates.LatitudeBeANumber)
            .Must(BeValidLatitude).WithMessage(Messages.Coordinates.LatitudeBeAValidNumber);

        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage(Messages.Coordinates.LongitudeObligatory)
            .Must(BeAValidDouble).WithMessage(Messages.Coordinates.LongitudeBeANumber)
            .Must(BeValidLongitude).WithMessage(Messages.Coordinates.LongitudeBeAValidNumber);
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

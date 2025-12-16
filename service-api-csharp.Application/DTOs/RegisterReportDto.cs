
namespace service_api_csharp.Application.DTOs;

public class RegisterReportDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string IdCategory { get; set; }
    private string _emergencyLevel;
    public string EmergencyLevel
    {
        get => _emergencyLevel;
        set => _emergencyLevel = NormalizeString(value);
    }

    private string NormalizeString(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
        var stringBuilder = new System.Text.StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLowerInvariant();
    }
    public DateTime DateReport { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string DirectionReport { get; set; }
    public string? ImageUrl { get; set; }
}
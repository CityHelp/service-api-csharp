namespace service_api_csharp.Application.DTOs;

public class RegisterReportDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string IdCategory { get; set; }
    public string EmergencyLevel { get; set; }
    public DateOnly DateReport { get; set; }
    public string DocumentUser { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string DirectionReports { get; set; }
    public ICollection<string> ListUrlImages { get; set; }
}
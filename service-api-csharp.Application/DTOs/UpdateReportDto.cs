namespace service_api_csharp.Application.DTOs;

public class UpdateReportDto
{
    public Guid ReportId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

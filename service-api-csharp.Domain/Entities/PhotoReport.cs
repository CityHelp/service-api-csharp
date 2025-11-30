using System;

namespace service_api_csharp.Domain.Entities;

public class PhotoReport
{
    public int Id { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public Guid ReportId { get; set; }

    // Navigation properties
    public Report Report { get; set; } = null!;
}
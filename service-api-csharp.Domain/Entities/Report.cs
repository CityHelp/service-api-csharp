using NetTopologySuite.Geometries;


namespace service_api_csharp.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Point UbicationCoordinates { get; set; } = null!;
    public string UbicationDirection { get; set; }
    public DateTime DateReport { get; set; }
    public string EmergencyLevel { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int NumDeleteReportRequest { get; set; }
    public List<int>? DeleteRequestUserIds { get; set; } = new();

    
    public int UserId { get; set; }
    // Navigation properties
    public User User { get; set; } = null!;
    
    public int CategoryId { get; set; }
    public CategoryReport Category { get; set; } = null!;
    public PhotoReport? Photo { get; set; }
}
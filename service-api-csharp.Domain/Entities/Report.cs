using System;
using System.Collections.Generic;
using service_api_csharp.Domain.ValueObjects;
using NetTopologySuite.Geometries;


namespace service_api_csharp.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Point UbicationCoordinates { get; set; } = null!; // ValueObject
    public DateTime DateReport { get; set; }
    public string EmergencyLevel { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int NumDeleteReportRequest { get; set; }
    public List<Guid> DeleteRequestUserIds { get; set; } = new();

    // Navigation properties
    public User User { get; set; } = null!;
    public CategoryReport Category { get; set; } = null!;
    public ICollection<PhotoReport> Photos { get; set; } = new List<PhotoReport>();
}
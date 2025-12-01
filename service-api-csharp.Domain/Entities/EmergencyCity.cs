using service_api_csharp.Domain.ValueObjects;

namespace service_api_csharp.Domain.Entities;

public class EmergencyCity
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public Point UbicationCoordinates { get; set; } = null!; // ValueObject
    public string UbicationDirection { get; set; } = string.Empty;
    public int IdSector { get; set; }

    // Navigation properties
    public CitySector Sector { get; set; } = null!;
}
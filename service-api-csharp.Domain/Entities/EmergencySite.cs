using service_api_csharp.Domain.ValueObjects;

namespace service_api_csharp.Domain.Entities;

public class EmergencySite
{
    public int Id { get; set; }
    public string NameSite { get; set; }
    public string Phone { get; set; } = string.Empty;
    public Point UbicationCoordinates { get; set; } = null!; // ValueObject
    public string UbicationDirection { get; set; } = string.Empty;
    public string Description { get; set; }
    public int SectorId { get; set; }
    public int CategoryId { get; set; }

    // Navigation properties
    public CitySector? Sector { get; set; } = null!;
    public ICollection<Cuadrante>? Cuadrantes { get; set; } = null;
    public EmergencySiteCategories? Category { get; set; } = null;
}
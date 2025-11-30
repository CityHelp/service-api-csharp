using System.Collections.Generic;
using service_api_csharp.Domain.ValueObjects;

namespace service_api_csharp.Domain.Entities;

public class CitySector
{
    public int Id { get; set; }
    public string NamePlace { get; set; } = string.Empty;
    public Polygon Area { get; set; } = null!; // ValueObject

    // Navigation properties
    public ICollection<EmergencySite> EmergencyCities { get; set; } = new List<EmergencySite>();
}

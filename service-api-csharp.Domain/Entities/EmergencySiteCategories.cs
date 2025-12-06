namespace service_api_csharp.Domain.Entities;

public class EmergencySiteCategories
{
    public int Id { get; set; }
    public string Category { get; set; }

    public ICollection<EmergencySite>? EmergencySites { get; set; } = null;
}
namespace service_api_csharp.Domain.Entities;

public class Cuadrante
{
    public int Id { get; set; }
    public string NumberCuadrante { get; set; }
    public string Phone { get; set; } = string.Empty;
    public int PoliceCAIId { get; set; }

    // Navigation properties
    public EmergencySite EmergencySite { get; set; } = null!;
}
namespace service_api_csharp.Application.DTOs;

public class EmergencySiteDto
{
    public string NameSite { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public UbicationUserDto Coordinates { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
}

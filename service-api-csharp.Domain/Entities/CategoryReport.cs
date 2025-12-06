using System.Collections.Generic;

namespace service_api_csharp.Domain.Entities;

public class CategoryReport
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}

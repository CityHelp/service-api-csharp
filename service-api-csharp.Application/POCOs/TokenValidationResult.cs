using System.Security.Claims;

namespace service_api_csharp.Application.POCOs;

public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public ClaimsPrincipal? Principal { get; set; }
    public string? ErrorMessage { get; set; }
}
using Microsoft.AspNetCore.Http;

namespace service_api_csharp.Application.POCOs;

public class ValidateFileCloudinaryPOCO
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public ICollection<string>  Errors { get; set; }
    
    public ValidateFileCloudinaryPOCO()
    {
    }

    public static ValidateFileCloudinaryPOCO Ok(object? data = null, string? message = null)
        => new() { Success = true };

    public static ValidateFileCloudinaryPOCO Fail(string message, params string[] errors)
        => new() { Success = false, Errors = errors, Message = message};
}
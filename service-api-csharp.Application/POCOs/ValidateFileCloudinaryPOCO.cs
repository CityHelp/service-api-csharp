using Microsoft.AspNetCore.Http;

namespace service_api_csharp.Application.POCOs;

public class ValidateFileCloudinary
{
    public bool Success { get; set; }
    public ICollection<string>  Errors { get; set; }
    
    public ValidateFileCloudinary()
    {
    }

    public static ValidateFileCloudinary Ok(object? data = null, string? message = null)
        => new() { Success = true };

    public static ValidateFileCloudinary Fail(string message, params string[] errors)
        => new() { Success = false, Errors = errors };
}
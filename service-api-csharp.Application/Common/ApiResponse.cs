namespace service_api_csharp.Application.Common;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
    public IEnumerable<string> Errors { get; set; }

    public ApiResponse()
    {
    }

    public static ApiResponse Ok(object? data = null, string? message = null)
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse Fail(string message, params string[] errors)
        => new() { Success = false, Message = message, Errors = errors };
}
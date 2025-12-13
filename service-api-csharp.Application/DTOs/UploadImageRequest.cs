using Microsoft.AspNetCore.Http;

namespace service_api_csharp.Application.DTOs;

public class UploadImageRequest
{
    public IFormFile File { get; set; }
}
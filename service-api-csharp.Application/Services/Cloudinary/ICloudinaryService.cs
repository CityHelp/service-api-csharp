using Microsoft.AspNetCore.Http;
using service_api_csharp.Application.Common;

namespace service_api_csharp.Application.Services.Cloudinary;

public interface ICloudinaryService
{
    Task<ApiResponse> UploadImageAsync(IFormFile file);
}

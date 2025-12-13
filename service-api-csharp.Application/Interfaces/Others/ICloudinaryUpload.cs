using Microsoft.AspNetCore.Http;

namespace service_api_csharp.Application.Common.RepositoriesInterfaces.Others;

public interface ICloudinaryUpload
{
    Task<string?> UploadFile(IFormFile file);
}
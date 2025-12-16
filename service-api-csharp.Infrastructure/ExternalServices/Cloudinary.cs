using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;

namespace service_api_csharp.Infrastructure.ExternalServices;

public class Cloudinary : ICloudinaryUpload
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;
    private readonly ILogger<Cloudinary> _logger;

    public Cloudinary(CloudinaryDotNet.Cloudinary cloudinary, ILogger<Cloudinary> logger)
    {
        _cloudinary = cloudinary;
        _logger = logger;
    }
    
    public async Task<string?> UploadFile(IFormFile file)
    {
        try
        {
            await using var stream = file.OpenReadStream();
            
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "images-reports",
                // Opcional: Puedes agregar transformaciones si lo necesitas
                // Transformation = new Transformation().Width(1920).Quality("auto")
            };


            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                _logger.LogError(
                    "Error de Cloudinary al subir archivo: {ErrorMessage}", 
                    uploadResult.Error.Message
                );
                return null;
            }

            _logger.LogInformation(
                "Archivo subido exitosamente a Cloudinary. URL: {Url}, PublicId: {PublicId}",
                uploadResult.SecureUrl,
                uploadResult.PublicId
            );

            return uploadResult.SecureUrl.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al subir archivo a Cloudinary: {FileName}", file.FileName);
            return null;
        }
    }
}
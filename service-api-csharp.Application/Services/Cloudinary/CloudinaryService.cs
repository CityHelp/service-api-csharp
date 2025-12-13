using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;
using service_api_csharp.Application.POCOs;

namespace service_api_csharp.Application.Services.Cloudinary;

public class CloudinaryService : ICloudinaryService
{
    private readonly ICloudinaryUpload _cloudinaryUpload;
    private readonly ILogger<CloudinaryService> _logger;

    // Configuración de validaciones
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public CloudinaryService(
        ICloudinaryUpload cloudinaryUpload,
        ILogger<CloudinaryService> logger)
    {
        _cloudinaryUpload = cloudinaryUpload;
        _logger = logger;
    }

    public async Task<ApiResponse> UploadImageAsync(IFormFile file)
    {
        // Validar que se haya enviado un archivo
        var validationResult = ValidateFile(file);
        if (!validationResult.Success)
        {
            List<string> errors = new List<string>();
            foreach (var error in validationResult.Errors)
            {
                errors.Add(error);
            }
            
            return ApiResponse.Fail(validationResult.Message, errors[0]);
        }

        try
        {
            // Subir el archivo a Cloudinary
            var imageUrl = await _cloudinaryUpload.UploadFile(file);

            if (string.IsNullOrEmpty(imageUrl))
            {
                _logger.LogError("Error al subir el archivo a Cloudinary");
                return ApiResponse.Fail(Messages.Errors.UnexpectedError);
            }

            _logger.LogInformation("Imagen subida exitosamente: {Url}", imageUrl);

            return ApiResponse.Ok(Messages.Clodinary.ImageUploadSucesfully,  imageUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al procesar la subida de imagen");
            return ApiResponse.Fail(Messages.Errors.UnexpectedError);
        }
    }

    private  ValidateFileCloudinaryPOCO ValidateFile(IFormFile file)
    {
        // Validar que el archivo no sea nulo o vacío
        if (file == null || file.Length == 0)
        {
            return ValidateFileCloudinaryPOCO.Fail(Messages.Clodinary.NoFileProvided);
        }

        // Validar extensión del archivo
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(fileExtension))
        {
            _logger.LogWarning("Tipo de archivo no permitido: {Extension}", fileExtension);

            return ValidateFileCloudinaryPOCO.Fail(
                Messages.Clodinary.TypeFileNotAllowed, fileExtension,
                $"Tipo de archivo no permitido. Solo se aceptan: {string.Join(", ", AllowedExtensions)}"
            );
        }

        // Validar tamaño del archivo
        if (file.Length > MaxFileSize)
        {
            _logger.LogWarning("Archivo demasiado grande: {Size} bytes", file.Length);
            return ValidateFileCloudinaryPOCO.Fail(
                Messages.Clodinary.FileTooBig);
        }

        // Todas las validaciones pasaron
        return ValidateFileCloudinaryPOCO.Ok(true);
    }
}

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

    // File validation configuration
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
        // Validate that a file has been sent
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
            // Upload the file to Cloudinary
            var imageUrl = await _cloudinaryUpload.UploadFile(file);

            if (string.IsNullOrEmpty(imageUrl))
            {
                _logger.LogError("Error uploading file to Cloudinary");
                return ApiResponse.Fail(Messages.Errors.UnexpectedError);
            }

            _logger.LogInformation("Image uploaded successfully: {Url}", imageUrl);

            return ApiResponse.Ok(Messages.Cloudinary.ImageUploadSuccessfully,  imageUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while processing image upload");
            return ApiResponse.Fail(Messages.Errors.UnexpectedError);
        }
    }

    private ValidateFileCloudinaryPOCO ValidateFile(IFormFile file)
    {
        // Validate that the file is not null or empty
        if (file == null || file.Length == 0)
        {
            return ValidateFileCloudinaryPOCO.Fail(Messages.Cloudinary.NoFileProvided);
        }

        // Validate file extension
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(fileExtension))
        {
            _logger.LogWarning("File type not allowed: {Extension}", fileExtension);

            return ValidateFileCloudinaryPOCO.Fail(
                Messages.Cloudinary.FileTypeNotAllowed, fileExtension,
                $"File type not allowed. Only the following formats are accepted: {string.Join(", ", AllowedExtensions)}"
            );
        }

        // Validate file size
        if (file.Length > MaxFileSize)
        {
            _logger.LogWarning("File size too large: {Size} bytes", file.Length);
            return ValidateFileCloudinaryPOCO.Fail(
                Messages.Cloudinary.FileTooBig);
        }

        // All validations passed
        return ValidateFileCloudinaryPOCO.Ok(true);
    }
}

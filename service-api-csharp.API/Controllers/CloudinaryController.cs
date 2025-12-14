using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;
using service_api_csharp.Application.Services.Cloudinary;

namespace service_api_csharp.API.Controllers;

[ApiController]
[Route("cloudinary")]
public class CloudinaryController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;
    
    public CloudinaryController(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest file)
    {
        var result = await _cloudinaryService.UploadImageAsync(file.File);
        
        if (!result.Success)
        {
            if (result.Message == Messages.Errors.UnexpectedError)
            {
                return BadRequest(result);
            }
        }

        return Ok(result);
    }
}
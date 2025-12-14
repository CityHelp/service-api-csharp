using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Application.Services;

namespace service_api_csharp.API.Controllers;

[ApiController]
[Authorize]
[Route("directories")]
public class SystemDirectoryController : ControllerBase
{
    private readonly ISystemDirectories _systemDirectories;
    private readonly ILogger<SystemDirectoryController> _logger;

    public SystemDirectoryController(ISystemDirectories systemDirectories, ILogger<SystemDirectoryController> logger)
    {
        _systemDirectories = systemDirectories;
        _logger = logger;
    }
    
    [Authorize]
    [HttpPost("emergency-sites-nearby")]
    public async Task<IActionResult> GetAllDirectories([FromBody] UbicationUserDto request)
    {
        _logger.LogInformation("Initiating request to get nearby emergency sites for coordinates: Lat {Latitude}, Lon {Longitude}", request.Latitude, request.Longitude);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for GetAllDirectories request: {Errors}", ModelState);
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _systemDirectories.GetEmergencySitesNearUbication(request);

            if (!response.Success)
            {
                _logger.LogWarning("Failed to retrieve emergency sites. Message: {Message}", response.Message);

                if (response.Message == Messages.Errors.UnexpectedError)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                return BadRequest(response);
            }

            _logger.LogInformation("Successfully retrieved nearby emergency sites.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while getting nearby emergency sites.");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail("An unexpected error occurred."));
        }
    }
}
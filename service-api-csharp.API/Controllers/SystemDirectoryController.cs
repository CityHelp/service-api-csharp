using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_api_csharp.Application.DTOs;
using service_api_csharp.Application.InterfacesRepositories;

namespace service_api_csharp.API.Controllers;

[ApiController]
[Route("directories")]
public class SystemDirectoryController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public SystemDirectoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    [HttpPost("get-all-directories")]
    public async Task<IActionResult> GetAllDirectories([FromBody] UbicationUserDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var sites = await _unitOfWork.SystemDirectories.GetEmergencySitesNearUbication(request);

        var result = sites.Select(s => new EmergencySiteDto
        {
            NameSite = s.NameSite,
            Phone = s.Phone,
            Address = s.UbicationDirection,
            Coordinates = new UbicationUserDto 
            { 
                Latitude = s.UbicationCoordinates.Y.ToString(), 
                Longitude = s.UbicationCoordinates.X.ToString() 
            },
            Description = s.Description,
            Category = s.Category?.Category ?? "Unknown"
        }).ToList();

        return Ok(result);
    }
}
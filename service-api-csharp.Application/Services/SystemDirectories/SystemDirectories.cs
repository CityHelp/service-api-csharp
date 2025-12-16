using Microsoft.Extensions.Logging;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Application.Services;

public class SystemDirectories : ISystemDirectories
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SystemDirectories> _logger;

    public SystemDirectories(IUnitOfWork unitOfWork, ILogger<SystemDirectories> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<ApiResponse> GetEmergencySitesNearUbication(UbicationUserDto ubication)
    {
        try
        {
            _logger.LogInformation("Getting emergency sites near location: Lat {Latitude}, Lon {Longitude}", 
                ubication.Latitude, ubication.Longitude);

            var response = await _unitOfWork.SystemDirectories.GetEmergencySitesNearUbication(ubication);

            var emergencySites = response.Select(site => new EmergencySiteDto
            {
                NameSite = site.NameSite,
                Phone = site.Phone,
                Address = site.UbicationDirection,
                Description = site.Description,
                Category = site.Category?.Category ?? "Unknown",

                Coordinates = new UbicationUserDto
                {
                    Latitude = site.UbicationCoordinates.Y.ToString(),
                    Longitude = site.UbicationCoordinates.X.ToString()
                }
            }).ToList();
            
            _logger.LogInformation("Successfully retrieved {Count} emergency sites near location", emergencySites.Count);
            
            return ApiResponse.Ok(Messages.SystemDirectory.FoundDirectories ,emergencySites);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting emergency sites near location: Lat {Latitude}, Lon {Longitude}", 
                ubication.Latitude, ubication.Longitude);
            
            return ApiResponse.Fail(Messages.Errors.UnexpectedError);
        }
    }
}
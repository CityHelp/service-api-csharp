using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Application.Services;

public class SystemDirectories : ISystemDirectories
{
    private readonly IUnitOfWork _unitOfWork;

    public SystemDirectories(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ApiResponse> GetEmergencySitesNearUbication(UbicationUserDto ubication)
    {
        try
        {
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
            
            return ApiResponse.Ok(emergencySites, Messages.SystemDirectory.FoundDirectories);
        }
        catch (Exception e)
        {
            return ApiResponse.Fail(Messages.Errors.UnexpectedError);
        }
    }
}
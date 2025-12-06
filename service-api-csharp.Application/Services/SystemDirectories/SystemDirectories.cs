using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Application.Services;

public class SystemDirectories : ISystemDirectories
{
    public Task<ApiResponse> GetEmergencySitesNearUbication(UbicationUserDto ubication)
    {
        throw new NotImplementedException();
    }
}
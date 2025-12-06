using service_api_csharp.Application.DTOs;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Application.InterfacesRepositories;

public interface ISystemDirectoriesRepository
{
    Task<ICollection<EmergencySite>> GetEmergencySitesNearUbication(UbicationUserDto ubication);
}
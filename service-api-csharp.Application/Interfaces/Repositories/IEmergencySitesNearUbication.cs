using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Application.Common.RepositoriesInterfaces;

public interface IEmergencySitesNearUbication
{
    Task<ICollection<EmergencySite>> FindEmergencySitesNearUbication();
    Task<ICollection<EmergencySite>> GetEmergencySitesGeneral();
}
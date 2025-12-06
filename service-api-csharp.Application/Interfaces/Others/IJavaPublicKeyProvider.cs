using Microsoft.IdentityModel.Tokens;

namespace service_api_csharp.Application.Common.RepositoriesInterfaces.Others;

public interface IJavaPublicKeyProvider
{ 
    Task<IEnumerable<SecurityKey>> GetPublicKeysAsync();
}

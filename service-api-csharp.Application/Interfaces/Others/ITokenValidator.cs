using System.Security.Claims;
using service_api_csharp.Application.POCOs;

namespace service_api_csharp.Application.Common.RepositoriesInterfaces.Others;

public interface ITokenValidator
{
    Task<TokenValidationResult> ValidateTokenAsync(string token);
}

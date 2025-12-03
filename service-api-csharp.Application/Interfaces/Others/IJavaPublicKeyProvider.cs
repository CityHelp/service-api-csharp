namespace service_api_csharp.Application.Common.RepositoriesInterfaces.Others;

public interface IJavaPublicKeyProvider
{ 
    Task<string> GetPublicKeyAsync();
}

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;
using Microsoft.IdentityModel.Tokens;
using service_api_csharp.Infrastructure.Helpers;

namespace service_api_csharp.Infrastructure.ExternalServices;

public class JavaPublicKeyProvider : IJavaPublicKeyProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "JavaPublicKey";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public JavaPublicKeyProvider(
        HttpClient httpClient,
        IMemoryCache cache,
        IOptions<AuthSettings> options)
    {
        _httpClient = httpClient;   
        _cache = cache;
    }

    public async Task<IEnumerable<SecurityKey>> GetPublicKeysAsync()
    {
        string? jwksJson = null;
        
        // Intentar obtener el JWKS desde el caché
        if (!_cache.TryGetValue<string>(CacheKey, out jwksJson) || string.IsNullOrWhiteSpace(jwksJson))
        {
            try
            {
                var response = await _httpClient.GetAsync("");
                response.EnsureSuccessStatusCode();

                jwksJson = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jwksJson))
                {
                    throw new InvalidOperationException("JWKS not found");
                }

                // Cachear el JWKS por 30 minutos
                _cache.Set(CacheKey, jwksJson, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration
                });
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("An error ocurrered while obtain JWKS", ex);
            }
        }

        // Parsear el JWKS y retornar las llaves
        var keySet = new JsonWebKeySet(jwksJson!);
        return keySet.GetSigningKeys();
    }
}

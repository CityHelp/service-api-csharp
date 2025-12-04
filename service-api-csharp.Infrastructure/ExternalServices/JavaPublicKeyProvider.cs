using Microsoft.Extensions.Caching.Memory;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;
using Microsoft.IdentityModel.Tokens;

namespace service_api_csharp.Infrastructure.ExternalServices;

public class JavaPublicKeyProvider : IJavaPublicKeyProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "JavaPublicKey";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public JavaPublicKeyProvider(
        HttpClient httpClient,
        IMemoryCache cache)
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
                var response = await _httpClient.GetAsync("/api/auth/public-key");
                response.EnsureSuccessStatusCode();

                jwksJson = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jwksJson))
                {
                    throw new InvalidOperationException("El JWKS recibido está vacío");
                }

                // Cachear el JWKS por 30 minutos
                _cache.Set(CacheKey, jwksJson, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration
                });
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("No se pudo obtener el JWKS del servicio de autenticación", ex);
            }
        }

        // Parsear el JWKS y retornar las llaves
        var keySet = new JsonWebKeySet(jwksJson!);
        return keySet.GetSigningKeys();
    }
}

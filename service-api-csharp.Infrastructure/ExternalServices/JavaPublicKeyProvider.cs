using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;
using Microsoft.IdentityModel.Tokens;
using service_api_csharp.Infrastructure.Helpers;

namespace service_api_csharp.Infrastructure.ExternalServices;

public class JavaPublicKeyProvider : IJavaPublicKeyProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<JavaPublicKeyProvider> _logger;
    private const string CacheKey = "JavaPublicKey";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public JavaPublicKeyProvider(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<JavaPublicKeyProvider> logger,
        IOptions<AuthSettings> options)
    {
        _httpClient = httpClient;   
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<SecurityKey>> GetPublicKeysAsync()
    {
        string? jwksJson = null;
        
        // Intentar obtener el JWKS desde el caché
        if (!_cache.TryGetValue<string>(CacheKey, out jwksJson) || string.IsNullOrWhiteSpace(jwksJson))
        {
            _logger.LogInformation("JWKS not found in cache, fetching from Java service");
            
            try
            {
                var response = await _httpClient.GetAsync("");
                response.EnsureSuccessStatusCode();

                jwksJson = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jwksJson))
                {
                    _logger.LogError("JWKS response was empty or null");
                    throw new InvalidOperationException("JWKS not found");
                }

                // Cachear el JWKS por 30 minutos
                _cache.Set(CacheKey, jwksJson, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration
                });
                
                _logger.LogInformation("JWKS successfully fetched and cached for {Duration} minutes", CacheDuration.TotalMinutes);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while obtaining JWKS from Java service");
                throw new InvalidOperationException("An error ocurrered while obtain JWKS", ex);
            }
        }
        else
        {
            _logger.LogInformation("JWKS retrieved from cache");
        }

        // Parsear el JWKS y retornar las llaves
        var keySet = new JsonWebKeySet(jwksJson!);
        return keySet.GetSigningKeys();
    }
}

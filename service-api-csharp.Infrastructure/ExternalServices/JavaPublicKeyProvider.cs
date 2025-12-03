using Microsoft.Extensions.Caching.Memory;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;

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

    public async Task<string> GetPublicKeyAsync()
    {
        // Intentar obtener la llave desde el caché
        if (_cache.TryGetValue(CacheKey, out string? cachedKey) && cachedKey != null)
        {
            return cachedKey;
        }

        try
        {
            var response = await _httpClient.GetAsync("/api/auth/public-key");
            response.EnsureSuccessStatusCode();

            var publicKey = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new InvalidOperationException("La llave pública recibida está vacía");
            }

            // Cachear la llave pública por 30 minutos para evitar llamadas frecuentes
            _cache.Set(CacheKey, publicKey, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            });
                        
            return publicKey;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("No se pudo obtener la llave pública del servicio de autenticación", ex);
        }
    }
}

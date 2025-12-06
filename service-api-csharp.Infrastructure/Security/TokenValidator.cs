using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;

namespace service_api_csharp.Infrastructure.Security;

public class TokenValidator : ITokenValidator
{
    private readonly IJavaPublicKeyProvider _publicKeyProvider;
    private readonly ILogger<TokenValidator> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public TokenValidator(
        IJavaPublicKeyProvider publicKeyProvider,
        ILogger<TokenValidator> logger)
    {
        _publicKeyProvider = publicKeyProvider;
        _logger = logger;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public async Task<service_api_csharp.Application.POCOs.TokenValidationResult> ValidateTokenAsync(string token)
    {
        try
        {
            // Obtener las llaves públicas (JWKS) desde el proveedor
            var signingKeys = await _publicKeyProvider.GetPublicKeysAsync();

            // Configurar los parámetros de validación
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5) // Tolerancia de 5 minutos para diferencias de reloj
            };

            // Validar el token
            var principal = _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = true,
                Principal = principal,
                ErrorMessage = null
            };
        }
        catch (SecurityTokenExpiredException)
        {
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = false,
                Principal = null,
                ErrorMessage = "El token ha expirado"
            };
        }
        catch (SecurityTokenException)
        {
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = false,
                Principal = null,
                ErrorMessage = "Token inválido"
            };
        }
        catch (Exception)
        {
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = false,
                Principal = null,
                ErrorMessage = "Error al validar el token"
            };
        }
    }
}

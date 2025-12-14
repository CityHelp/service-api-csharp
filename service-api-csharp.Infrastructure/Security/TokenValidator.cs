using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;
using Microsoft.Extensions.Options;
using service_api_csharp.Infrastructure.Helpers;
using System.Security.Claims;

namespace service_api_csharp.Infrastructure.Security;

public class TokenValidator : ITokenValidator
{
    private readonly IJavaPublicKeyProvider _publicKeyProvider;
    private readonly AuthSettings _authSettings;
    private readonly ILogger<TokenValidator> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public TokenValidator(
        IJavaPublicKeyProvider publicKeyProvider,
        IOptions<AuthSettings> authSettings,
        ILogger<TokenValidator> logger)
    {
        _publicKeyProvider = publicKeyProvider;
        _authSettings = authSettings.Value;
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
                
                ValidateIssuer = !string.IsNullOrEmpty(_authSettings.Issuer),
                ValidIssuer = _authSettings.Issuer,
                
                ValidateAudience = !string.IsNullOrEmpty(_authSettings.Audience),
                ValidAudience = _authSettings.Audience,
                
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5) // Tolerancia de 5 minutos para diferencias de reloj
            };

            // Validar el token
            var principal = _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            // Log claims for debugging
            foreach (var claim in principal.Claims)
            {
                _logger.LogDebug("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }
            
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = true,
                Principal = principal,
                ErrorMessage = null
            };
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogWarning(ex, "Token validation failed: Token has expired");
            
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = false,
                Principal = null,
                ErrorMessage = "El token ha expirado"
            };
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "Token validation failed: Invalid token");
            
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = false,
                Principal = null,
                ErrorMessage = "Token inválido"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while validating token");
            
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = false,
                Principal = null,
                ErrorMessage = "Error al validar el token"
            };
        }
    }
}

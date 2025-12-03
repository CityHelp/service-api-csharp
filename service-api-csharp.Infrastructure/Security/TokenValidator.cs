using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using service_api_csharp.Application.Common.RepositoriesInterfaces;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;
using service_api_csharp.Application.POCOs;

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
            // Obtener la llave pública desde el proveedor
            var publicKeyPem = await _publicKeyProvider.GetPublicKeyAsync();

            // Convertir la llave pública PEM a RsaSecurityKey
            var rsaSecurityKey = ConvertPemToRsaSecurityKey(publicKeyPem);

            // Configurar los parámetros de validación
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = rsaSecurityKey,
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
        catch (SecurityTokenExpiredException ex)
        {
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = false,
                Principal = null,
                ErrorMessage = "El token ha expirado"
            };
        }
        catch (SecurityTokenException ex)
        {
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = false,
                Principal = null,
                ErrorMessage = "Token inválido"
            };
        }
        catch (Exception ex)
        {
            return new service_api_csharp.Application.POCOs.TokenValidationResult
            {
                IsValid = false,
                Principal = null,
                ErrorMessage = "Error al validar el token"
            };
        }
    }
    
    private RsaSecurityKey ConvertPemToRsaSecurityKey(string publicKeyPem)
    {
        try
        {
            // Limpiar el formato PEM (remover headers y saltos de línea)
            var pemContent = publicKeyPem
                .Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "")
                .Trim();

            // Decodificar desde Base64
            var keyBytes = Convert.FromBase64String(pemContent);

            // Crear RSA desde los bytes
            var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);

            return new RsaSecurityKey(rsa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir la llave pública PEM a RsaSecurityKey");
            throw new InvalidOperationException("No se pudo procesar la llave pública", ex);
        }
    }
}

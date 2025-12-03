using service_api_csharp.Application.Common.RepositoriesInterfaces;
using service_api_csharp.Application;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;

namespace service_api_csharp.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITokenValidator tokenValidator)
    {
        var token = ExtractTokenFromHeader(context);

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var validationResult = await tokenValidator.ValidateTokenAsync(token);

                if (validationResult.IsValid && validationResult.Principal != null)
                {
                    // Asignar el ClaimsPrincipal al contexto
                    context.User = validationResult.Principal;
                        
                }
                else
                {
                    _logger.LogWarning("Token inválido: {Error}", validationResult.ErrorMessage);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        error = "Unauthorized", 
                        message = validationResult.ErrorMessage ?? "Token inválido" 
                    });
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar el token JWT");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new 
                { 
                    error = "Unauthorized", 
                    message = "Error al procesar el token de autenticación" 
                });
                return;
            }
        }
        else
        {
            _logger.LogWarning("No se encontró token de autenticación en el header");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new 
            { 
                error = "Unauthorized", 
                message = "Token de autenticación requerido" 
            });
            return;
        }

        await _next(context);
    }

    /// <summary>
    /// Extrae el token JWT del header Authorization
    /// </summary>
    private string? ExtractTokenFromHeader(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader))
        {
            return null;
        }

        // El formato esperado es: "Bearer {token}"
        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        return null;
    }
}

using service_api_csharp.API.Middleware;

namespace service_api_csharp.API.Extensions;

/// <summary>
/// Extensiones para registrar middlewares personalizados
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Agrega el middleware de validación JWT al pipeline
    /// </summary>
    public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app)
    {
        return app.UseMiddleware<JwtMiddleware>();
    }
}

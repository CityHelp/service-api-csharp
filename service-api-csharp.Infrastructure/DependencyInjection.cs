using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;
using service_api_csharp.Infrastructure.ExternalServices;
using service_api_csharp.Infrastructure.Persistence;
using service_api_csharp.Infrastructure.Security;

namespace service_api_csharp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Database
        var connectionString = Environment.GetEnvironmentVariable("DefaultConnection__ConnectionStrings");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Missing connection string ❌.");
        }

        try
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    x => x.UseNetTopologySuite()
                )
            );
            
            Console.WriteLine("Database connected ✅");

        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred with the database ❌");
            Console.WriteLine(e.Message);
            throw;
        }
        

        #endregion
        
        // ========== JWT Authentication Services ==========
        
        // Agregar caché en memoria
        services.AddMemoryCache();

        // Configurar HttpClient para el macroservicio Java
        var javaServiceBaseUrl = Environment.GetEnvironmentVariable("JavaAuthService__BaseUrl");
        
        if (string.IsNullOrEmpty(javaServiceBaseUrl))
        {
            throw new InvalidOperationException("Variable de entorno 'JavaAuthService__BaseUrl' no encontrada");
        }

        services.AddHttpClient<IJavaPublicKeyProvider, JavaPublicKeyProvider>(client =>
        {
            client.BaseAddress = new Uri(javaServiceBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Registrar el validador de tokens
        services.AddScoped<ITokenValidator, TokenValidator>();
        
        return services;
    }
}
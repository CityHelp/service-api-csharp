using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Application.Services;
using service_api_csharp.Infrastructure.ExternalServices;
using service_api_csharp.Infrastructure.Persistence;
using service_api_csharp.Infrastructure.Repositories;
using service_api_csharp.Infrastructure.Security;

namespace service_api_csharp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //Database service
        #region Database
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        
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
        
        //Jwt Service
        #region Jwt
        // ========== JWT Authentication Services ==========
        
        // Agregar caché en memoria
        services.AddMemoryCache();
        
        // Configurar HttpClient para el macroservicio Java
        var url = configuration["JavaAuthService__Url"];
        
        if (string.IsNullOrEmpty(url))
        {
            throw new InvalidOperationException("Environment variable 'JavaAuthService__Url' not found");
        }
        services.AddHttpClient<IJavaPublicKeyProvider, JavaPublicKeyProvider>(client =>
        {
             client.BaseAddress = new Uri(url);
        });
        #endregion
        
        //Dependencies
        
        services.AddScoped<ITokenValidator, TokenValidator>();
        services.AddScoped<ISystemDirectoriesRepository, SystemDirectoriesRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
        
        return services;
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Infrastructure.ExternalServices;
using service_api_csharp.Infrastructure.Persistence;
using service_api_csharp.Infrastructure.Repositories;
using service_api_csharp.Infrastructure.Security;
using service_api_csharp.Infrastructure.Helpers;

namespace service_api_csharp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //Database service
        #region Database
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
     
        
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
            
            // Create a temporary service provider to get logger for configuration logging
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<AppDbContext>>();
            logger.LogInformation("Database connection configured successfully ✅");
        }
        catch (Exception ex)
        {
            // Create a temporary service provider to get logger for error logging
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<AppDbContext>>();
            logger.LogError(ex, "An error occurred while configuring the database connection ❌");
            throw;
        }
        

        #endregion
        
        //Jwt Service
        #region Jwt
        // ========== JWT Authentication Services ==========
        
        // Agregar caché en memoria
        
        // Configurar HttpClient para el macroservicio Java
        var url = configuration["Auth:JavaUrl"];
        
        if (string.IsNullOrEmpty(url))
        {
            throw new InvalidOperationException("Environment variable 'Auth__JavaUrl' not found");
        }
        services.AddHttpClient<IJavaPublicKeyProvider, JavaPublicKeyProvider>(client =>
        {
             client.BaseAddress = new Uri(url);
        });
        #endregion
        
        //Configuration Java
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));
        
        //Configuration cloudinary
        // 1. Cargar las configuraciones de Cloudinary
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

        // 2. Crear y registrar la instancia de Cloudinary como Singleton
        services.AddSingleton(sp => {
            // Obtener las credenciales del sistema de configuración
            var config = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

            // Crear y devolver el objeto Account
            var account = new CloudinaryDotNet.Account(
                config.CloudName,
                config.ApiKey,
                config.ApiSecret
            );
    
            // Crear y devolver el objeto Cloudinary principal
            return new CloudinaryDotNet.Cloudinary(account);
        });
        
        //Services
        services.AddMemoryCache();
        services.AddScoped<ITokenValidator, TokenValidator>();
        services.AddScoped<ISystemDirectoriesRepository, SystemDirectoriesRepository>();
        services.AddScoped<IReportsRepository, ReportsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICloudinaryUpload, CloudinaryUpload>();
        services.AddScoped<service_api_csharp.Application.Services.Cloudinary.ICloudinaryService, service_api_csharp.Application.Services.Cloudinary.CloudinaryService>();
        services.AddSingleton(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
        
        return services;
    }
}
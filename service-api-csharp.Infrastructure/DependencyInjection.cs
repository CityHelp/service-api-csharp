using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using service_api_csharp.Infrastructure.Persistence;

namespace service_api_csharp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DefaultConnection__ConnectionStrings"] ?? configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Missing connection string ❌");
        }

        try
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    x => x.UseNetTopologySuite() // <--- register NetPologySuite
                )
            );
            
            Console.WriteLine("Database conected connected ✅");

        }
        catch (Exception e)
        {
            Console.WriteLine("An error ocurred with the database ❌");
            Console.WriteLine(e.Message);
            throw;
        }
        
        return services;
    }
}
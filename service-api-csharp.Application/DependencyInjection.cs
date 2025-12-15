using Microsoft.Extensions.DependencyInjection;
using service_api_csharp.Application.Services;

namespace service_api_csharp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddScoped<ISystemDirectories, SystemDirectories>();
        services.AddScoped<IReportsService, ReportsService>();
        return services;
    }
}
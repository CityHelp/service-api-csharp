using Microsoft.Extensions.DependencyInjection;

namespace service_api_csharp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }
}
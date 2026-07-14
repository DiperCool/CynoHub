using CynoHub.Application.Interfaces.Services;
using CynoHub.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CynoHub.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILitterService, LitterService>();

        return services;
    }
}

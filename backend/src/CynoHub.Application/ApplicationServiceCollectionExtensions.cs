using CynoHub.Application.EventHandlers;
using CynoHub.Application.Interfaces.Services;
using CynoHub.Application.Services;
using CynoHub.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace CynoHub.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILitterService, LitterService>();
        services.AddScoped<
            IDomainEventHandler<LitterPublishedEvent>,
            LitterPublishedEventHandler
        >();

        return services;
    }
}

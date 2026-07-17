using System.Text.Json;
using CynoHub.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CynoHub.Infrastructure.Services;

public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task DispatchAsync(
        string eventTypeStr,
        string eventContent,
        CancellationToken cancellationToken
    )
    {
        Type? eventType = Type.GetType(eventTypeStr);
        if (eventType == null)
            return;

        var domainEvent = JsonSerializer.Deserialize(eventContent, eventType);
        if (domainEvent == null)
            return;

        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
        var handlers = serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("HandleAsync");
            if (method != null)
            {
                var task = (Task)method.Invoke(handler, [domainEvent, cancellationToken])!;
                await task;
            }
        }
    }
}

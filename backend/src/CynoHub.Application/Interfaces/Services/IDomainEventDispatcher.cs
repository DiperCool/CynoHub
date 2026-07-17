namespace CynoHub.Application.Interfaces.Services;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(string eventType, string eventContent, CancellationToken cancellationToken);
}

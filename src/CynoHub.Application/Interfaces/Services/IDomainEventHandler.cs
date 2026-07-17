using CynoHub.Domain.Common;

namespace CynoHub.Application.Interfaces.Services;

public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
}

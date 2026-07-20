using CynoHub.Application.Interfaces.Services;
using CynoHub.Domain.Events;

namespace CynoHub.Application.EventHandlers;

public class LitterPublishedEventHandler(IEventPublisher eventPublisher)
    : IDomainEventHandler<LitterPublishedEvent>
{
    public async Task HandleAsync(
        LitterPublishedEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        await eventPublisher.PublishLitterPublishedEventAsync(
            domainEvent.BreederId,
            domainEvent.LitterId,
            cancellationToken
        );
    }
}

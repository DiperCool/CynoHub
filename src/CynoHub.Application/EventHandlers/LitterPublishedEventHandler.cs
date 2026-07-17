using System.Threading;
using System.Threading.Tasks;
using CynoHub.Application.Interfaces.Services;
using CynoHub.Domain.Events;

namespace CynoHub.Application.EventHandlers;

public class LitterPublishedEventHandler(INotificationService notificationService) 
    : IDomainEventHandler<LitterPublishedEvent>
{
    public async Task HandleAsync(LitterPublishedEvent domainEvent, CancellationToken cancellationToken)
    {
        await notificationService.SendPublishedNotificationAsync(
            domainEvent.BreederId, 
            domainEvent.LitterId, 
            cancellationToken);
    }
}

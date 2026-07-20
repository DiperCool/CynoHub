namespace CynoHub.Application.Interfaces.Services;

public interface IEventPublisher
{
    Task PublishLitterPublishedEventAsync(Guid breederId, Guid litterId, CancellationToken ct = default);
}

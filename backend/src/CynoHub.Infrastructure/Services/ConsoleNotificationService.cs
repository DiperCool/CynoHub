using CynoHub.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CynoHub.Infrastructure.Services;

public sealed class ConsoleNotificationService(ILogger<ConsoleNotificationService> logger)
    : INotificationService
{
    public Task SendPublishedNotificationAsync(
        Guid breederId,
        Guid litterId,
        CancellationToken ct = default
    )
    {
        logger.LogInformation(
            "[Notification] Litter {LitterId} published by breeder {BreederId}. Simulated email sent.",
            litterId,
            breederId
        );

        return Task.CompletedTask;
    }
}

namespace CynoHub.Application.Interfaces.Services;

/// <summary>
/// Simulates sending email notifications (console output only).
/// </summary>
public interface INotificationService
{
    Task SendPublishedNotificationAsync(
        Guid breederId,
        Guid litterId,
        CancellationToken ct = default
    );
}

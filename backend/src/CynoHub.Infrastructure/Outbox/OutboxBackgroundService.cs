using CynoHub.Application.Interfaces.Services;
using CynoHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CynoHub.Infrastructure.Outbox;

public class OutboxBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<OutboxBackgroundService> logger
) : BackgroundService
{
    private const int PollingIntervalMilliseconds = 10000;
    private const int BatchSize = 20;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox messages.");
            }

            // Polling interval
            await Task.Delay(PollingIntervalMilliseconds, stoppingToken);
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();

        var messages = await dbContext
            .OutboxMessages.Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(BatchSize)
            .ToListAsync(stoppingToken);

        if (messages.Count == 0)
            return;

        foreach (var message in messages)
        {
            try
            {
                await dispatcher.DispatchAsync(message.Type, message.Content, stoppingToken);

                message.ProcessedOn = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                message.Error = ex.ToString();
            }
        }

        await dbContext.SaveChangesAsync(stoppingToken);
    }
}

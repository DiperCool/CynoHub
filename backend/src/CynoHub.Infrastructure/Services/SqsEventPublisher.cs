using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using CynoHub.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace CynoHub.Infrastructure.Services;

public class SqsEventPublisher(IAmazonSQS sqsClient, IConfiguration configuration) : IEventPublisher
{
    private readonly string _queueUrl =
        configuration["AWS:SqsQueueUrl"]
        ?? throw new ArgumentNullException("AWS:SqsQueueUrl missing in configuration");

    public async Task PublishLitterPublishedEventAsync(
        Guid breederId,
        Guid litterId,
        CancellationToken ct = default
    )
    {
        var messageBody = JsonSerializer.Serialize(
            new
            {
                EventType = "LitterPublished",
                BreederId = breederId,
                LitterId = litterId,
                Timestamp = DateTime.UtcNow,
            }
        );

        var request = new SendMessageRequest { QueueUrl = _queueUrl, MessageBody = messageBody };

        await sqsClient.SendMessageAsync(request, ct);
    }
}

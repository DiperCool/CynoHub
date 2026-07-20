using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

namespace CynoHub.NotificationLambda.Services;

public class DynamoDbEventProcessor(AmazonDynamoDBClient dynamoDbClient) : IEventProcessor
{
    public async Task ProcessAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation(
            $"[DynamoDbEventProcessor] Processing message {message.MessageId}"
        );

        var request = new PutItemRequest
        {
            TableName = "LitterEventLogs",
            Item = new Dictionary<string, AttributeValue>
            {
                {
                    "Id",
                    new AttributeValue { S = Guid.NewGuid().ToString() }
                },
                {
                    "MessageId",
                    new AttributeValue { S = message.MessageId }
                },
                {
                    "Body",
                    new AttributeValue { S = message.Body }
                },
                {
                    "ReceivedAt",
                    new AttributeValue { S = DateTime.UtcNow.ToString("O") }
                },
            },
        };

        try
        {
            await dynamoDbClient.PutItemAsync(request);
            context.Logger.LogInformation(
                $"[DynamoDbEventProcessor] Successfully saved message {message.MessageId} to DynamoDB."
            );
        }
        catch (Exception ex)
        {
            context.Logger.LogError(
                $"[DynamoDbEventProcessor] Error saving to DynamoDB: {ex.Message}"
            );
            throw; // Rethrow to make SQS retry or send to DLQ
        }
    }
}

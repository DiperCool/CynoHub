using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using CynoHub.NotificationLambda.Services;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda JSON serializer.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CynoHub.NotificationLambda;

public class Function
{
    private readonly IServiceProvider _serviceProvider;

    public Function()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // 1. Configure AWS DynamoDB client
        var endpointUrl = Environment.GetEnvironmentVariable("AWS_ENDPOINT_URL");
        if (string.IsNullOrEmpty(endpointUrl))
        {
            var localstackHost = Environment.GetEnvironmentVariable("LOCALSTACK_HOSTNAME") ?? "localhost";
            endpointUrl = $"http://{localstackHost}:4566";
        }
        
        var config = new AmazonDynamoDBConfig { ServiceURL = endpointUrl };
        services.AddSingleton(new AmazonDynamoDBClient(config));

        // 2. Register Business Logic
        services.AddScoped<IEventProcessor, DynamoDbEventProcessor>();
    }

    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        // Create a new scope per Lambda invocation
        using var scope = _serviceProvider.CreateScope();
        
        // Resolve our processors (or other dependencies)
        var processor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

        foreach (var message in evnt.Records)
        {
            try 
            {
                await processor.ProcessAsync(message, context);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error processing message {message.MessageId}: {ex.Message}");
                throw; // Rethrow to trigger SQS retry mechanism
            }
        }
    }
}

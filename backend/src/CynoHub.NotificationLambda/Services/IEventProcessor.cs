using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

namespace CynoHub.NotificationLambda.Services;

public interface IEventProcessor
{
    Task ProcessAsync(SQSEvent.SQSMessage message, ILambdaContext context);
}

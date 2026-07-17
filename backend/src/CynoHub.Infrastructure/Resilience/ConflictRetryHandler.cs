using CynoHub.Application.Interfaces.Services;
using Polly.Registry;

namespace CynoHub.Infrastructure.Resilience;

public class ConflictRetryHandler(ResiliencePipelineProvider<string> pipelineProvider)
    : IConflictRetryHandler
{
    public async Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        CancellationToken ct = default
    )
    {
        var pipeline = pipelineProvider.GetPipeline("PublishLitter");
        await pipeline.ExecuteAsync(async cancellationToken => await action(cancellationToken), ct);
    }
}

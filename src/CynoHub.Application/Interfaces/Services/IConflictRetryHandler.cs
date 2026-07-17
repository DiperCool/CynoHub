using System;
using System.Threading;
using System.Threading.Tasks;

namespace CynoHub.Application.Interfaces.Services;

public interface IConflictRetryHandler
{
    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
}

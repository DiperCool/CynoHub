using CynoHub.Domain.Entities;
using CynoHub.Domain.Interfaces.Repositories;
using CynoHub.Infrastructure.Persistence;

namespace CynoHub.Infrastructure.Repositories;

public sealed class AuditLogRepository(AppDbContext db) : IAuditLogRepository
{
    public async Task AddAsync(AuditLog log, CancellationToken ct = default) =>
        await db.AuditLogs.AddAsync(log, ct);
}

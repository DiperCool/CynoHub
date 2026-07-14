using CynoHub.Domain.Entities;

namespace CynoHub.Domain.Interfaces.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log, CancellationToken ct = default);
}

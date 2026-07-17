using CynoHub.Domain.Entities;
using CynoHub.Domain.Enums;
using CynoHub.Domain.Interfaces.Repositories;
using CynoHub.Infrastructure.Extensions;
using CynoHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CynoHub.Infrastructure.Repositories;

public sealed class LitterRepository(AppDbContext db) : ILitterRepository
{
    public Task<Litter?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Litters.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<(IReadOnlyList<Litter> Items, int TotalCount)> GetPagedByBreederAsync(
        Guid breederId,
        LitterStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var query = db.Litters.Where(l => l.BreederId == breederId);

        if (status.HasValue)
            query = query.Where(l => l.Status == status.Value);

        return await query
            .OrderByDescending(l => l.CreatedAt)
            .ToPagedAsync(pageNumber, pageSize, ct);
    }

    public async Task AddAsync(Litter litter, CancellationToken ct = default) =>
        await db.Litters.AddAsync(litter, ct);

    public void Update(Litter litter) => db.Litters.Update(litter);
}

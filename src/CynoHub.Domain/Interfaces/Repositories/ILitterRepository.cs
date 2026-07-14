using CynoHub.Domain.Entities;
using CynoHub.Domain.Enums;

namespace CynoHub.Domain.Interfaces.Repositories;

public interface ILitterRepository
{
    Task<Litter?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Litter> Items, int TotalCount)> GetPagedByBreederAsync(
        Guid breederId,
        LitterStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default
    );
    Task AddAsync(Litter litter, CancellationToken ct = default);
    void Update(Litter litter);
}

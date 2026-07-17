using CynoHub.Domain.Entities;
using CynoHub.Domain.Interfaces.Repositories;
using CynoHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CynoHub.Infrastructure.Repositories;

public sealed class BreederBenefitRepository(AppDbContext db) : IBreederBenefitRepository
{
    public Task<BreederBenefit?> GetByBreederIdAsync(
        Guid breederId,
        CancellationToken ct = default
    ) => db.BreederBenefits.FirstOrDefaultAsync(b => b.BreederId == breederId, ct);

    public async Task AddAsync(BreederBenefit benefit, CancellationToken ct = default) =>
        await db.BreederBenefits.AddAsync(benefit, ct);

    public void Update(BreederBenefit benefit) => db.BreederBenefits.Update(benefit);
}

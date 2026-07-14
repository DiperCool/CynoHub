using CynoHub.Domain.Entities;

namespace CynoHub.Domain.Interfaces.Repositories;

public interface IBreederBenefitRepository
{
    Task<BreederBenefit?> GetByBreederIdAsync(Guid breederId, CancellationToken ct = default);
    Task AddAsync(BreederBenefit benefit, CancellationToken ct = default);
    void Update(BreederBenefit benefit);
}

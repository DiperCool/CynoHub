using CynoHub.Application.DTOs;
using CynoHub.Domain.Entities;

namespace CynoHub.Application.Mappings;

public static class LitterMappings
{
    public static LitterDto ToDto(this Litter litter) =>
        new(litter.Id, litter.BreederId, litter.Status, litter.CreatedAt);

    public static IReadOnlyList<LitterDto> ToDtoList(this IEnumerable<Litter> litters) =>
        litters.Select(l => l.ToDto()).ToList().AsReadOnly();
}

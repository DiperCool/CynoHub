using CynoHub.Domain.Exceptions;

namespace CynoHub.Domain.Entities;

public class BreederBenefit
{
    /// <summary>Required by EF Core, do not use directly.</summary>
    private BreederBenefit() { }

    public BreederBenefit(Guid breederId, int freeLimit, int usedCount = 0)
    {
        BreederId = breederId;
        FreeLimit = freeLimit;
        UsedCount = usedCount;
    }

    public Guid BreederId { get; private set; }
    public int FreeLimit { get; private set; }
    public int UsedCount { get; private set; }
    public byte[] Version { get; private set; } = [];

    public bool HasFreeSlots => UsedCount < FreeLimit;

    /// <summary>
    /// Increments <see cref="UsedCount"/> by one.
    /// Throws <see cref="DomainException"/> if the limit is already exhausted.
    /// Caller is responsible for persisting the audit log BEFORE calling this
    /// when the "failed attempt" path is needed.
    /// </summary>
    public void ConsumeSlot()
    {
        if (!HasFreeSlots)
            throw new DomainException(
                $"Breeder '{BreederId}' has exhausted all {FreeLimit} free publication slot(s). "
                    + "Upgrade your plan to continue publishing."
            );

        UsedCount++;
    }
}

using CynoHub.Domain.Common;
using CynoHub.Domain.Enums;
using CynoHub.Domain.Exceptions;
using CynoHub.Domain.Events;

namespace CynoHub.Domain.Entities;

public class Litter : Entity
{
    /// <summary>Required by EF Core — do not use directly.</summary>
    private Litter() { }

    public Litter(Guid id, Guid breederId, LitterStatus status, DateTime createdAt)
    {
        Id = id;
        BreederId = breederId;
        Status = status;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid BreederId { get; private set; }
    public LitterStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public byte[] Version { get; private set; } = [];

    /// <summary>
    /// Guards both ownership and status in one call.
    /// Throws <see cref="ForbiddenException"/> or <see cref="DomainException"/> on violation.
    /// </summary>
    public void EnsureCanBePublishedBy(Guid breederId)
    {
        if (BreederId != breederId)
            throw new ForbiddenException(
                $"Breeder '{breederId}' is not the owner of Litter '{Id}'."
            );

        if (Status != LitterStatus.Approved)
            throw new DomainException(
                $"Litter '{Id}' cannot be published: current status is '{Status}'. "
                    + "Only 'Approved' litters are eligible for publishing."
            );
    }

    public void MarkPublished()
    {
        Status = LitterStatus.Published;
        AddDomainEvent(new LitterPublishedEvent(Id, BreederId));
    }
}

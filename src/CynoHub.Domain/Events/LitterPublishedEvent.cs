using System;
using CynoHub.Domain.Common;

namespace CynoHub.Domain.Events;

public sealed record LitterPublishedEvent(Guid LitterId, Guid BreederId) : IDomainEvent;

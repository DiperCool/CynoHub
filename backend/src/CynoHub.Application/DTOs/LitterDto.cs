using CynoHub.Domain.Enums;

namespace CynoHub.Application.DTOs;

public record LitterDto(Guid Id, Guid BreederId, LitterStatus Status, DateTime CreatedAt);

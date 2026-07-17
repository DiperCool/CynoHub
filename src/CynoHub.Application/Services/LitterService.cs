using CynoHub.Application.Common.Pagination;
using CynoHub.Application.DTOs;
using CynoHub.Application.Interfaces.Services;
using CynoHub.Application.Mappings;
using CynoHub.Domain.Constants;
using CynoHub.Domain.Entities;
using CynoHub.Domain.Enums;
using CynoHub.Domain.Exceptions;
using CynoHub.Domain.Interfaces.Repositories;

namespace CynoHub.Application.Services;

public sealed class LitterService(
    ILitterRepository litterRepository,
    IBreederBenefitRepository benefitRepository,
    IAuditLogRepository auditLogRepository,
    IUnitOfWork unitOfWork,
    INotificationService notificationService,
    IBreederService breederService
) : ILitterService
{
    public async Task PublishAsync(Guid litterId, CancellationToken ct = default)
    {
        var litter =
            await litterRepository.GetByIdAsync(litterId, ct)
            ?? throw new NotFoundException(nameof(Litter), litterId);

        var breederId = breederService.CurrentBreederId 
            ?? throw new UnauthorizedException("Breeder authentication required.");

        litter.EnsureCanBePublishedBy(breederId);

        var benefit =
            await benefitRepository.GetByBreederIdAsync(breederId, ct)
            ?? throw new NotFoundException(nameof(BreederBenefit), breederId);

        if (!benefit.HasFreeSlots)
        {
            await LogAsync(litterId, AuditLogActions.PublishAttemptFailedLimitExceeded, ct);
            await unitOfWork.SaveChangesAsync(ct);

            throw new DomainException("Free limits exhausted.");
        }

        try
        {
            await unitOfWork.ExecuteInTransactionAsync(
                async () =>
                {
                    benefit.ConsumeSlot();
                    benefitRepository.Update(benefit);

                    litter.MarkPublished();
                    litterRepository.Update(litter);

                    await auditLogRepository.AddAsync(
                        CreateLog(litterId, AuditLogActions.PublishedForFree),
                        ct
                    );
                },
                ct
            );
        }
        catch (ConflictException)
        {
            // TODO: add conflict logging
            throw;
        }

        // TODO: Implement Transactional Outbox Pattern here
        // Sending notifications directly outside the transaction can lead to message loss if the process crashes after the DB commit.
        // We should write an OutboxMessage to the DB inside the transaction above, and process it in a background worker.
        await notificationService.SendPublishedNotificationAsync(breederId, litterId, ct);
    }

    public async Task<PagedResult<LitterDto>> GetPagedAsync(
        LitterStatus? status,
        PaginationQuery pagination,
        CancellationToken ct = default
    )
    {
        var p = pagination.Normalize();

        var breederId = breederService.CurrentBreederId 
            ?? throw new UnauthorizedException("Breeder authentication required.");

        var (items, totalCount) = await litterRepository.GetPagedByBreederAsync(
            breederId,
            status,
            p.PageNumber,
            p.PageSize,
            ct
        );

        return PagedResult<LitterDto>.Create(
            items.ToDtoList(),
            totalCount,
            p.PageNumber,
            p.PageSize
        );
    }

    private Task LogAsync(Guid litterId, string action, CancellationToken ct) =>
        auditLogRepository.AddAsync(CreateLog(litterId, action), ct);

    private static AuditLog CreateLog(Guid litterId, string action) =>
        new()
        {
            Id = Guid.NewGuid(),
            EntityId = litterId,
            Action = action,
            CreatedAt = DateTime.UtcNow,
        };
}

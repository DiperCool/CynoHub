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
    IBreederService breederService,
    IConflictRetryHandler retryHandler
) : ILitterService
{
    public async Task PublishAsync(Guid litterId, CancellationToken ct = default)
    {
        await retryHandler.ExecuteAsync(async cancellationToken =>
            {
                var litter =
                    await litterRepository.GetByIdAsync(litterId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Litter), litterId);

                var breederId =
                    breederService.CurrentBreederId
                    ?? throw new UnauthorizedException("Breeder authentication required.");

                litter.EnsureCanBePublishedBy(breederId);

                var benefit =
                    await benefitRepository.GetByBreederIdAsync(breederId, cancellationToken)
                    ?? throw new NotFoundException(nameof(BreederBenefit), breederId);

                if (!benefit.HasFreeSlots)
                {
                    await LogAsync(
                        litterId,
                        AuditLogActions.PublishAttemptFailedLimitExceeded,
                        cancellationToken
                    );
                    await unitOfWork.SaveChangesAsync(cancellationToken);

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
                                cancellationToken
                            );
                        },
                        cancellationToken
                    );
                }
                catch (ConflictException)
                {
                    // TODO: add conflict logging
                    throw;
                }
            },
            ct
        );
    }

    public async Task<PagedResult<LitterDto>> GetPagedAsync(
        LitterStatus? status,
        PaginationQuery pagination,
        CancellationToken ct = default
    )
    {
        var p = pagination.Normalize();

        var breederId =
            breederService.CurrentBreederId
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

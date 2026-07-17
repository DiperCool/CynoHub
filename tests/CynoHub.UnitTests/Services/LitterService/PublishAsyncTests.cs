using CynoHub.Domain.Constants;
using CynoHub.Domain.Entities;
using CynoHub.Domain.Enums;
using CynoHub.Domain.Exceptions;
using Moq;

namespace CynoHub.UnitTests.Services.LitterService;

public sealed class PublishAsyncTests : LitterServiceTestsBase
{
    [Fact]
    public async Task Publish_WhenLitterNotFound_ThrowsNotFoundException()
    {
        var (svc, litterRepo, _, _, _, _, _) = CreateSut();
        litterRepo.Setup(r => r.GetByIdAsync(LitterId, default)).ReturnsAsync((Litter?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => svc.PublishAsync(LitterId));
    }

    [Fact]
    public async Task Publish_WhenBreederIsNotOwner_ThrowsForbiddenException()
    {
        var (svc, litterRepo, _, _, _, _, _) = CreateSut();
        var differentOwner = Guid.NewGuid();
        litterRepo
            .Setup(r => r.GetByIdAsync(LitterId, default))
            .ReturnsAsync(MakeLitter(breederId: differentOwner));

        await Assert.ThrowsAsync<ForbiddenException>(() => svc.PublishAsync(LitterId));
    }

    [Theory]
    [InlineData(LitterStatus.Draft)]
    [InlineData(LitterStatus.Submitted)]
    [InlineData(LitterStatus.Published)]
    public async Task Publish_WhenStatusIsNotApproved_ThrowsDomainException(
        LitterStatus wrongStatus
    )
    {
        var (svc, litterRepo, _, _, _, _, _) = CreateSut();
        litterRepo
            .Setup(r => r.GetByIdAsync(LitterId, default))
            .ReturnsAsync(MakeLitter(wrongStatus));

        await Assert.ThrowsAsync<DomainException>(() => svc.PublishAsync(LitterId));
    }

    [Fact]
    public async Task Publish_WhenBenefitNotFound_ThrowsNotFoundException()
    {
        var (svc, litterRepo, benefitRepo, _, _, _, _) = CreateSut();
        litterRepo.Setup(r => r.GetByIdAsync(LitterId, default)).ReturnsAsync(MakeLitter());
        benefitRepo
            .Setup(r => r.GetByBreederIdAsync(BreederId, default))
            .ReturnsAsync((BreederBenefit?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => svc.PublishAsync(LitterId));
    }

    [Fact]
    public async Task Publish_WhenLimitExceeded_LogsAttemptAndThrowsDomainException()
    {
        var (svc, litterRepo, benefitRepo, auditRepo, uow, _, _) = CreateSut();

        litterRepo.Setup(r => r.GetByIdAsync(LitterId, default)).ReturnsAsync(MakeLitter());
        benefitRepo
            .Setup(r => r.GetByBreederIdAsync(BreederId, default))
            .ReturnsAsync(MakeBenefit(freeLimit: 3, usedCount: 3));



        var ex = await Assert.ThrowsAsync<DomainException>(
            () => svc.PublishAsync(LitterId)
        );

        Assert.Contains("exhausted", ex.Message);

        auditRepo.Verify(
            r =>
                r.AddAsync(
                    It.Is<AuditLog>(l =>
                        l.Action == AuditLogActions.PublishAttemptFailedLimitExceeded
                    ),
                    default
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Publish_WhenLimitAvailable_PublishesAndIncreasesUsedCount()
    {
        var (svc, litterRepo, benefitRepo, auditRepo, uow, notifications, _) = CreateSut();

        var litter = MakeLitter();
        var benefit = MakeBenefit(freeLimit: 3, usedCount: 1);

        litterRepo.Setup(r => r.GetByIdAsync(LitterId, default)).ReturnsAsync(litter);
        benefitRepo.Setup(r => r.GetByBreederIdAsync(BreederId, default)).ReturnsAsync(benefit);


        await svc.PublishAsync(LitterId);

        Assert.Equal(LitterStatus.Published, litter.Status);
        Assert.Equal(2, benefit.UsedCount);

        notifications.Verify(
            n => n.SendPublishedNotificationAsync(BreederId, LitterId, default),
            Times.Once
        );
        auditRepo.Verify(
            r =>
                r.AddAsync(
                    It.Is<AuditLog>(l => l.Action == AuditLogActions.PublishedForFree),
                    default
                ),
            Times.Once
        );
    }
}

using CynoHub.Application.Interfaces.Services;
using CynoHub.Application.Services;
using CynoHub.Domain.Entities;
using CynoHub.Domain.Enums;
using CynoHub.Domain.Interfaces.Repositories;
using Moq;

namespace CynoHub.UnitTests.Services.LitterService;

public abstract class LitterServiceTestsBase
{
    protected static readonly Guid BreederId = Guid.NewGuid();
    protected static readonly Guid LitterId = Guid.NewGuid();

    protected static Litter MakeLitter(
        LitterStatus status = LitterStatus.Approved,
        Guid? breederId = null
    ) => new Litter(LitterId, breederId ?? BreederId, status, DateTime.UtcNow);

    protected static BreederBenefit MakeBenefit(int freeLimit = 3, int usedCount = 0) =>
        new BreederBenefit(BreederId, freeLimit, usedCount);

    protected static (
        CynoHub.Application.Services.LitterService svc,
        Mock<ILitterRepository> litterRepo,
        Mock<IBreederBenefitRepository> benefitRepo,
        Mock<IAuditLogRepository> auditRepo,
        Mock<IUnitOfWork> uow,
        Mock<INotificationService> notifications
    ) CreateSut()
    {
        var litterRepo = new Mock<ILitterRepository>();
        var benefitRepo = new Mock<IBreederBenefitRepository>();
        var auditRepo = new Mock<IAuditLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        var notifications = new Mock<INotificationService>();

        uow.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (action, _) => await action());

        var svc = new CynoHub.Application.Services.LitterService(
            litterRepo.Object,
            benefitRepo.Object,
            auditRepo.Object,
            uow.Object,
            notifications.Object
        );

        return (svc, litterRepo, benefitRepo, auditRepo, uow, notifications);
    }
}

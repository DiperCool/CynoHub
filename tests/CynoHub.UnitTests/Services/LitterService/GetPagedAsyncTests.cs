using CynoHub.Application.Common.Pagination;
using CynoHub.Domain.Entities;
using CynoHub.Domain.Enums;
using Moq;

namespace CynoHub.UnitTests.Services.LitterService;

public sealed class GetPagedAsyncTests : LitterServiceTestsBase
{
    [Fact]
    public async Task GetPaged_ReturnsCorrectPagedResult()
    {
        var (svc, litterRepo, _, _, _, _, _, _) = CreateSut();

        var litters = new List<Litter>
        {
            MakeLitter(LitterStatus.Published),
            MakeLitter(LitterStatus.Approved),
        };

        litterRepo
            .Setup(r => r.GetPagedByBreederAsync(BreederId, (LitterStatus?)null, 1, 10, default))
            .ReturnsAsync((litters.AsReadOnly() as IReadOnlyList<Litter>, litters.Count));

        var result = await svc.GetPagedAsync(
            null,
            new PaginationQuery { PageNumber = 1, PageSize = 10 }
        );

        Assert.Equal(2, result.Pagination.TotalCount);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(1, result.Pagination.TotalPages);
        Assert.False(result.Pagination.HasNextPage);
        Assert.False(result.Pagination.HasPreviousPage);
    }

    [Fact]
    public async Task GetPaged_FiltersByStatus_PassesCorrectFilterToRepository()
    {
        var (svc, litterRepo, _, _, _, _, _, _) = CreateSut();

        var filtered = new List<Litter> { MakeLitter(LitterStatus.Approved) };

        litterRepo
            .Setup(r => r.GetPagedByBreederAsync(BreederId, LitterStatus.Approved, 1, 10, default))
            .ReturnsAsync((filtered.AsReadOnly() as IReadOnlyList<Litter>, 1));

        var result = await svc.GetPagedAsync(
            LitterStatus.Approved,
            new PaginationQuery { PageNumber = 1, PageSize = 10 }
        );

        Assert.Single(result.Data);
        Assert.Equal(LitterStatus.Approved, result.Data[0].Status);
    }

    [Fact]
    public async Task GetPaged_ClampsPageSizeToMax100()
    {
        var (svc, litterRepo, _, _, _, _, _, _) = CreateSut();

        litterRepo
            .Setup(r => r.GetPagedByBreederAsync(BreederId, (LitterStatus?)null, 1, 100, default))
            .ReturnsAsync((new List<Litter>().AsReadOnly() as IReadOnlyList<Litter>, 0));

        await svc.GetPagedAsync(
            null,
            new PaginationQuery { PageNumber = 1, PageSize = 999 }
        );

        litterRepo.Verify(
            r => r.GetPagedByBreederAsync(BreederId, (LitterStatus?)null, 1, 100, default),
            Times.Once
        );
    }

    [Fact]
    public async Task GetPaged_ClampsPageNumberToMinimum1()
    {
        var (svc, litterRepo, _, _, _, _, _, _) = CreateSut();

        litterRepo
            .Setup(r => r.GetPagedByBreederAsync(BreederId, (LitterStatus?)null, 1, 10, default))
            .ReturnsAsync((new List<Litter>().AsReadOnly() as IReadOnlyList<Litter>, 0));

        await svc.GetPagedAsync(
            null,
            new PaginationQuery { PageNumber = -5, PageSize = 10 }
        );

        litterRepo.Verify(
            r => r.GetPagedByBreederAsync(BreederId, (LitterStatus?)null, 1, 10, default),
            Times.Once
        );
    }
}

using CynoHub.Domain.Constants;
using CynoHub.Domain.Entities;
using CynoHub.Domain.Enums;
using CynoHub.Infrastructure.Persistence;
using CynoHub.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CynoHub.IntegrationTests.Litters;

public sealed class LitterPublishStressTests(CynoHubWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private const int TotalLitters = 10;
    private const int FreeSlots = 2;
    private int ExpectedFailures => TotalLitters - FreeSlots;

    protected override async Task SeedAsync(AppDbContext db)
    {
        db.BreederBenefits.Add(new BreederBenefit(BreederId, freeLimit: FreeSlots, usedCount: 0));

        for (var i = 0; i < TotalLitters; i++)
        {
            db.Litters.Add(
                new Litter(Guid.NewGuid(), BreederId, LitterStatus.Approved, DateTime.UtcNow)
            );
        }

        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task ConcurrentPublish_ShouldRespectFreeSlotLimit()
    {
        await using var db = CreateDbContext();
        var litterIds = await db
            .Litters.Where(l => l.BreederId == BreederId)
            .Select(l => l.Id)
            .ToListAsync();

        var responses = await Task.WhenAll(
            litterIds.Select(id => Client.PostAsync($"/api/litters/{id}/publish", null))
        );

        var successCount = responses.Count(r => r.IsSuccessStatusCode);
        var failureCount = responses.Count(r => !r.IsSuccessStatusCode);

        Assert.Equal(FreeSlots, successCount);
        Assert.Equal(ExpectedFailures, failureCount);

        await using var verifyDb = CreateDbContext();

        var benefit = await verifyDb
            .BreederBenefits.AsNoTracking()
            .FirstAsync(b => b.BreederId == BreederId);
        Assert.Equal(FreeSlots, benefit.UsedCount);

        var publishedCount = await verifyDb
            .Litters.AsNoTracking()
            .CountAsync(l => l.Status == LitterStatus.Published && l.BreederId == BreederId);
        Assert.Equal(FreeSlots, publishedCount);

        var auditLogCount = await verifyDb
            .AuditLogs.AsNoTracking()
            .CountAsync(a => a.Action == AuditLogActions.PublishedForFree);
        Assert.Equal(FreeSlots, auditLogCount);
    }
}

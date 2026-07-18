using CynoHub.Infrastructure.Persistence;
using CynoHub.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CynoHub.IntegrationTests;

public abstract class IntegrationTestBase
    : IClassFixture<CynoHubWebApplicationFactory>, IAsyncLifetime
{
    protected readonly CynoHubWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected readonly Guid BreederId = Guid.NewGuid();

    protected IntegrationTestBase(CynoHubWebApplicationFactory factory)
    {
        Factory = factory;
        Client = Factory.CreateClient();
        Client.DefaultRequestHeaders.Add("X-Breeder-Id", BreederId.ToString());
    }

    public async Task InitializeAsync()
    {
        await using var db = CreateDbContext();
        await db.Database.EnsureCreatedAsync();
        await SeedAsync(db);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected virtual Task SeedAsync(AppDbContext db) => Task.CompletedTask;

    protected AppDbContext CreateDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}

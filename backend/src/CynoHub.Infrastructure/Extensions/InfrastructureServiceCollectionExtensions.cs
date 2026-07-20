using CynoHub.Application.Interfaces.Services;
using CynoHub.Domain.Entities;
using CynoHub.Domain.Enums;
using CynoHub.Domain.Exceptions;
using CynoHub.Domain.Interfaces.Repositories;
using CynoHub.Infrastructure.Outbox;
using CynoHub.Infrastructure.Persistence;
using CynoHub.Infrastructure.Repositories;
using CynoHub.Infrastructure.Resilience;
using CynoHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

namespace CynoHub.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> dbOptions
    )
    {
        services.AddDbContext<AppDbContext>(
            (provider, options) =>
            {
                dbOptions(options);
            }
        );

        // Repositories
        services.AddScoped<ILitterRepository, LitterRepository>();
        services.AddScoped<IBreederBenefitRepository, BreederBenefitRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // External services
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IEventPublisher, SqsEventPublisher>();

        services.AddSingleton<Amazon.SQS.IAmazonSQS>(sp =>
        {
            var configuration =
                sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            var serviceUrl = configuration["AWS:ServiceURL"] ?? "http://localhost:4566";
            var region = configuration["AWS:Region"] ?? "us-east-1";

            var sqsConfig = new Amazon.SQS.AmazonSQSConfig
            {
                ServiceURL = serviceUrl,
                AuthenticationRegion = region,
            };
            // LocalStack uses dummy credentials
            return new Amazon.SQS.AmazonSQSClient("test", "test", sqsConfig);
        });

        // Resilience
        services.AddScoped<IConflictRetryHandler, ConflictRetryHandler>();
        services.AddResiliencePipeline(
            "PublishLitter",
            builder =>
            {
                builder.AddRetry(
                    new RetryStrategyOptions
                    {
                        ShouldHandle = new PredicateBuilder().Handle<ConflictException>(),
                        Delay = TimeSpan.FromMilliseconds(100),
                        MaxRetryAttempts = 3,
                        BackoffType = DelayBackoffType.Exponential,
                    }
                );
            }
        );

        // Outbox Worker
        services.AddHostedService<OutboxBackgroundService>();

        return services;
    }

    /// <summary>
    /// Seeds baseline data for manual testing.
    /// Call this during app startup in development only.
    /// </summary>
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        // Seed breeder benefit if not present
        var testBreederId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        if (!await db.BreederBenefits.AnyAsync())
        {
            db.BreederBenefits.Add(new BreederBenefit(testBreederId, freeLimit: 3));
        }

        // Seed litters for all statuses
        if (!await db.Litters.AnyAsync())
        {
            db.Litters.AddRange(
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000010"),
                    breederId: testBreederId,
                    status: LitterStatus.Approved,
                    createdAt: DateTime.UtcNow.AddDays(-1)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000011"),
                    breederId: testBreederId,
                    status: LitterStatus.Draft,
                    createdAt: DateTime.UtcNow.AddDays(-4)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000012"),
                    breederId: testBreederId,
                    status: LitterStatus.Submitted,
                    createdAt: DateTime.UtcNow.AddDays(-3)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000013"),
                    breederId: testBreederId,
                    status: LitterStatus.Published,
                    createdAt: DateTime.UtcNow.AddDays(-2)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000014"),
                    breederId: testBreederId,
                    status: LitterStatus.Approved,
                    createdAt: DateTime.UtcNow.AddDays(-5)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000015"),
                    breederId: testBreederId,
                    status: LitterStatus.Approved,
                    createdAt: DateTime.UtcNow.AddDays(-6)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000016"),
                    breederId: testBreederId,
                    status: LitterStatus.Approved,
                    createdAt: DateTime.UtcNow.AddDays(-7)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000017"),
                    breederId: testBreederId,
                    status: LitterStatus.Draft,
                    createdAt: DateTime.UtcNow.AddDays(-10)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000018"),
                    breederId: testBreederId,
                    status: LitterStatus.Draft,
                    createdAt: DateTime.UtcNow.AddDays(-12)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000019"),
                    breederId: testBreederId,
                    status: LitterStatus.Submitted,
                    createdAt: DateTime.UtcNow.AddDays(-14)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000020"),
                    breederId: testBreederId,
                    status: LitterStatus.Published,
                    createdAt: DateTime.UtcNow.AddDays(-15)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000021"),
                    breederId: testBreederId,
                    status: LitterStatus.Approved,
                    createdAt: DateTime.UtcNow.AddDays(-18)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000022"),
                    breederId: testBreederId,
                    status: LitterStatus.Approved,
                    createdAt: DateTime.UtcNow.AddDays(-20)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000023"),
                    breederId: testBreederId,
                    status: LitterStatus.Submitted,
                    createdAt: DateTime.UtcNow.AddDays(-25)
                ),
                new Litter(
                    id: Guid.Parse("00000000-0000-0000-0000-000000000024"),
                    breederId: testBreederId,
                    status: LitterStatus.Published,
                    createdAt: DateTime.UtcNow.AddDays(-30)
                )
            );
        }

        await db.SaveChangesAsync();
    }

    public static async Task EnsureSqsQueueCreatedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var sqsClient = scope.ServiceProvider.GetRequiredService<Amazon.SQS.IAmazonSQS>();
        var configuration =
            scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();

        var queueUrl = configuration["AWS:SqsQueueUrl"];
        if (string.IsNullOrEmpty(queueUrl))
            return;

        var queueName = queueUrl.Split('/').Last();

        await sqsClient.CreateQueueAsync(
            new Amazon.SQS.Model.CreateQueueRequest { QueueName = queueName }
        );
    }
}

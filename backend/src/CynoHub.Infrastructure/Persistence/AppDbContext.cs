using CynoHub.Domain.Common;
using CynoHub.Domain.Entities;
using CynoHub.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CynoHub.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Litter> Litters => Set<Litter>();
    public DbSet<BreederBenefit> BreederBenefits => Set<BreederBenefit>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var versionProperty = entry.Properties.FirstOrDefault(p =>
                    p.Metadata.Name == "Version"
                );
                if (versionProperty != null)
                {
                    versionProperty.CurrentValue = Guid.NewGuid().ToByteArray();
                }
            }
        }

        var entitiesWithEvents = ChangeTracker.Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToList();
            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                OutboxMessages.Add(new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    OccurredOn = DateTime.UtcNow,
                    Type = domainEvent.GetType().AssemblyQualifiedName!,
                    Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
                });
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

using CynoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CynoHub.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Litter> Litters => Set<Litter>();
    public DbSet<BreederBenefit> BreederBenefits => Set<BreederBenefit>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

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

        return base.SaveChangesAsync(cancellationToken);
    }
}

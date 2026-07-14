using CynoHub.Domain.Entities;
using CynoHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CynoHub.Infrastructure.Persistence.Configurations;

public sealed class LitterConfiguration : IEntityTypeConfiguration<Litter>
{
    public void Configure(EntityTypeBuilder<Litter> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Status).HasConversion<string>().IsRequired();
        builder.Property(l => l.BreederId).IsRequired();
        builder.Property(l => l.CreatedAt).IsRequired();
        builder.Property(l => l.Version).IsConcurrencyToken();
    }
}

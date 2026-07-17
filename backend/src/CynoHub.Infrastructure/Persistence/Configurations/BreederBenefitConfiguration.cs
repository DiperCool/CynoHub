using CynoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CynoHub.Infrastructure.Persistence.Configurations;

public sealed class BreederBenefitConfiguration : IEntityTypeConfiguration<BreederBenefit>
{
    public void Configure(EntityTypeBuilder<BreederBenefit> builder)
    {
        builder.HasKey(b => b.BreederId);
        builder.Property(b => b.FreeLimit).IsRequired();
        builder.Property(b => b.UsedCount).IsRequired();
        builder.Property(b => b.Version).IsConcurrencyToken();
    }
}

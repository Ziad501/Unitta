using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unitta.Domain.Entities;

namespace Unitta.Infrastructure.Persistence.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("Features");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .UseIdentityColumn();

        builder.Property(f => f.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.Description)
            .HasMaxLength(500);

        builder.Property(f => f.UnitId)
            .IsRequired();

        builder.HasIndex(f => f.UnitId);

        builder.HasOne(f => f.Unit)
            .WithMany(u => u.Features)
            .HasForeignKey(f => f.UnitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
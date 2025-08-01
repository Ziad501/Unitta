using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unitta.Domain.Entities;

namespace Unitta.Infrastructure.Persistence.Configurations;

public class UnitNoConfiguration : IEntityTypeConfiguration<UnitNo>
{
    public void Configure(EntityTypeBuilder<UnitNo> builder)
    {
        builder.ToTable("UnitNumbers");

        builder.HasKey(un => un.UnitNumber);

        builder.Property(un => un.UnitNumber)
            .ValueGeneratedNever();

        builder.Property(un => un.UnitId)
            .IsRequired();

        builder.Property(un => un.SpecialDetails)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(un => un.UnitId);
        builder.HasIndex(un => new { un.UnitId, un.UnitNumber })
            .IsUnique();

        // Relationships
        builder.HasOne(un => un.Unit)
            .WithMany(u => u.UnitNumbers)
            .HasForeignKey(un => un.UnitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
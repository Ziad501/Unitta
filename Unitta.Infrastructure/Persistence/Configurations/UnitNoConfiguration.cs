using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unitta.Domain.Entities;

namespace Unitta.Infrastructure.Persistence.Configurations;

public class UnitNoConfiguration : IEntityTypeConfiguration<UnitNo>
{
    public void Configure(EntityTypeBuilder<UnitNo> builder)
    {
        builder.ToTable("UnitNumbers");

        builder.HasKey(un => un.Id); // Use Id as primary key

        builder.Property(un => un.Id)
            .UseIdentityAlwaysColumn();

        builder.Property(un => un.UnitNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(un => un.UnitId); // Nullable foreign key

        builder.Property(un => un.SpecialDetails)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(un => un.UnitNumber)
            .IsUnique(); // Each unit number must be unique

        builder.HasIndex(un => un.UnitId)
            .IsUnique(); // Each unit can have only one unit number

        // Relationships are configured from the Unit side (principal entity)
    }
}
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
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(un => un.UnitId);

        builder.Property(un => un.SpecialDetails)
            .HasMaxLength(500);

        builder.HasIndex(un => un.UnitId)
            .IsUnique();
    }
}
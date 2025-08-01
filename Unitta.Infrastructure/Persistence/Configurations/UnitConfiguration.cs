using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unitta.Domain.Entities;

namespace Unitta.Infrastructure.Persistence.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .UseIdentityAlwaysColumn();

        builder.Property(u => u.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Description)
            .HasMaxLength(1000);

        builder.Property(u => u.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(u => u.Sqft)
            .IsRequired();

        builder.Property(u => u.Occupancy)
            .IsRequired();

        builder.Property(u => u.ImageUrl)
            .HasMaxLength(500);

        builder.Property(u => u.CreatedDate)
            .HasColumnType("timestamp with time zone");

        builder.Property(u => u.UpdatedDate)
            .HasColumnType("timestamp with time zone");

        // Relationships
        builder.HasMany(u => u.Features)
            .WithOne(f => f.Unit)
            .HasForeignKey(f => f.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UnitNumbers)
            .WithOne(un => un.Unit)
            .HasForeignKey(un => un.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.Unit)
            .HasForeignKey(b => b.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
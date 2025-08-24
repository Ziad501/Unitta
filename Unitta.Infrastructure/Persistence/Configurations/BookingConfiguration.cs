using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Identity;

namespace Unitta.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .UseIdentityColumn();

        builder.Property(b => b.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(b => b.UnitId)
            .IsRequired();

        builder.Property(b => b.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(b => b.Phone)
            .HasMaxLength(30);

        builder.Property(b => b.TotalCost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(b => b.Nights)
            .IsRequired();

        builder.Property(b => b.Status)
            .HasMaxLength(50);

        builder.Property(b => b.BookingDate)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(b => b.CheckInDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(b => b.CheckOutDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(b => b.IsPaymentSuccessful)
            .HasDefaultValue(false);

        builder.Property(b => b.PaymentDate)
            .HasColumnType("datetimeoffset");

        builder.Property(b => b.StripeSessionId)
            .HasMaxLength(200);

        builder.Property(b => b.StripePaymentIntentId)
            .HasMaxLength(200);

        builder.Property(b => b.ActualCheckInDate)
            .HasColumnType("datetimeoffset");

        builder.Property(b => b.ActualCheckOutDate)
            .HasColumnType("datetimeoffset");

        builder.Property(b => b.UnitNumber)
            .IsRequired();
        builder.Ignore(b => b.UnitNoList);

        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.UnitId);
        builder.HasIndex(b => new { b.CheckInDate, b.CheckOutDate });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Unit)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Identity;
using Unitta.Infrastructure.Persistence.Configurations;

namespace Unitta.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{

    public DbSet<Unit> Units { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<UnitNo> UnitNumbers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ApplicationUserConfiguration());
        builder.ApplyConfiguration(new UnitConfiguration());
        builder.ApplyConfiguration(new BookingConfiguration());
        builder.ApplyConfiguration(new FeatureConfiguration());
        builder.ApplyConfiguration(new UnitNoConfiguration());
    }
}
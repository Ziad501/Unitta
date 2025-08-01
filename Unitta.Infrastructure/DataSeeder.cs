using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Identity;
using Unitta.Infrastructure.Persistence;

namespace Unitta.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        // Get required services
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Ensure the database is created
        await context.Database.EnsureCreatedAsync();

        // 2. Check if data already exists to prevent re-seeding
        if (await context.Units.AnyAsync())
        {
            return; // DB has been seeded
        }

        // 3. Seed a User
        var testUser = new ApplicationUser
        {
            UserName = "testuser@example.com",
            Email = "testuser@example.com",
            Name = "Test User",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };
        var userResult = await userManager.CreateAsync(testUser, "Password123!");
        if (!userResult.Succeeded)
        {
            throw new Exception("Could not create seed user.");
        }

        // 4. Seed Units with related Features and UnitNumbers
        var units = new List<Unit>();
        var faker = new Faker();

        for (int i = 1; i <= 10; i++)
        {
            var unit = new Unit
            {
                Name = $"Luxury Suite {i}",
                Description = faker.Lorem.Paragraph(3),
                Price = faker.Random.Double(150, 500),
                Sqft = faker.Random.Int(400, 1200),
                Occupancy = faker.Random.Int(2, 6),
                ImageUrl = $"https://picsum.photos/seed/{i}/600/400",
                CreatedDate = DateTime.UtcNow,
                Features = new List<Feature>
            {
                new() { Name = "Free Wi-Fi" },
                new() { Name = "Air Conditioning" },
                new() { Name = faker.PickRandom("Pool View", "City View", "Garden View") },
                new() { Name = faker.PickRandom("King Bed", "Queen Bed", "Two Double Beds") }
            },
                UnitNumbers = new List<UnitNo>()
            };

            // Add some physical unit numbers for this unit type
            for (int j = 1; j <= 3; j++)
            {
                unit.UnitNumbers.Add(new UnitNo
                {
                    // Generate a unique room number, e.g., 101, 102, 201, 202
                    UnitNumber = (i * 100) + j,
                    SpecialDetails = faker.PickRandom(null, "Corner room with extra window.")
                });
            }
            units.Add(unit);
        }
        await context.Units.AddRangeAsync(units);
        await context.SaveChangesAsync(); // Save units to get their IDs

        // 5. Seed Bookings for the user
        var seededUnits = await context.Units.Include(u => u.UnitNumbers).ToListAsync();
        var bookings = new List<Booking>
    {
        // Booking 1
        new()
        {
            UserId = testUser.Id,
            UnitId = seededUnits[0].Id,
            Name = testUser.Name,
            Email = testUser.Email,
            Phone = faker.Phone.PhoneNumber(),
            TotalCost = seededUnits[0].Price * 3,
            Nights = 3,
            Status = "Confirmed",
            BookingDate = DateTime.UtcNow.AddDays(-10),
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(8)),
            UnitNumber = seededUnits[0].UnitNumbers.First().UnitNumber,
            IsPaymentSuccessful = true,
            PaymentDate = DateTime.UtcNow.AddDays(-10),
            StripeSessionId = "stripe_session_id_1"
        },
        // Booking 2
        new()
        {
            UserId = testUser.Id,
            UnitId = seededUnits[2].Id,
            Name = testUser.Name,
            Email = testUser.Email,
            Phone = faker.Phone.PhoneNumber(),
            TotalCost = seededUnits[2].Price * 5,
            Nights = 5,
            Status = "Pending",
            BookingDate = DateTime.UtcNow.AddDays(-2),
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(20)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(25)),
            UnitNumber = seededUnits[2].UnitNumbers.First().UnitNumber,
            IsPaymentSuccessful = false,
        }
    };
        await context.Bookings.AddRangeAsync(bookings);
        await context.SaveChangesAsync();
    }
}
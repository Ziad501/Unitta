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

        var faker = new Faker();

        // --- FIX: Seeding is now a multi-step process ---

        // 4a. Seed Unit "Templates" (Room Types)
        var unitTemplates = new List<Unit>();
        for (int i = 1; i <= 4; i++) // Create 4 different room types
        {
            unitTemplates.Add(new Unit
            {
                Name = faker.PickRandom("Luxury Suite", "Deluxe King", "Standard Queen", "Executive Studio") + $" Type {i}",
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
                    new() { Name = faker.PickRandom("Pool View", "City View") }
                }
            });
        }
        await context.Units.AddRangeAsync(unitTemplates);
        await context.SaveChangesAsync(); // Save templates to get their database-generated IDs

        // 4b. Seed UnitNo "Physical Rooms" and link them to the templates
        var physicalRooms = new List<UnitNo>();
        foreach (var template in unitTemplates)
        {
            // Create 5 physical rooms for each room type
            for (int j = 1; j <= 5; j++)
            {
                physicalRooms.Add(new UnitNo
                {
                    UnitId = template.Id, // Link to the template
                    Unit = template, // Set the navigation property
                    UnitNumber = (template.Id * 100 + j), // e.g., 101, 102, 201, etc.
                    SpecialDetails = faker.PickRandom(null, "Corner room", "Recently renovated.")
                });
            }
        }
        await context.UnitNumbers.AddRangeAsync(physicalRooms);
        await context.SaveChangesAsync(); // Save the physical rooms

        // 5. Seed Bookings for the user
        var seededUnits = await context.Units.ToListAsync();
        var seededUnitNumbers = await context.UnitNumbers.ToListAsync();

        var firstUnitType = seededUnits.First();
        var thirdUnitType = seededUnits.Skip(2).First();

        // Find a physical room belonging to the first unit type
        var roomForBooking1 = seededUnitNumbers.First(un => un.UnitId == firstUnitType.Id);
        // Find a physical room belonging to the third unit type
        var roomForBooking2 = seededUnitNumbers.First(un => un.UnitId == thirdUnitType.Id);

        var bookings = new List<Booking>
        {
            // Booking 1
            new()
            {
                UserId = testUser.Id,
                UnitId = firstUnitType.Id, // The booking is for a 'Luxury Suite' type
                Name = testUser.Name,
                Email = testUser.Email,
                Phone = faker.Phone.PhoneNumber(),
                TotalCost = firstUnitType.Price * 3,
                Nights = 3,
                Status = "Confirmed",
                BookingDate = DateTime.UtcNow.AddDays(-10),
                CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(8)),
                // <-- FIX: Parse the string UnitNumber to the int required by Booking
                UnitNumber = roomForBooking1.UnitNumber,
                IsPaymentSuccessful = true,
                PaymentDate = DateTime.UtcNow.AddDays(-10),
                StripeSessionId = "stripe_session_id_1"
            },
            // Booking 2
            new()
            {
                UserId = testUser.Id,
                UnitId = thirdUnitType.Id, // The booking is for a 'Standard Queen' type
                Name = testUser.Name,
                Email = testUser.Email,
                Phone = faker.Phone.PhoneNumber(),
                TotalCost = thirdUnitType.Price * 5,
                Nights = 5,
                Status = "Pending",
                BookingDate = DateTime.UtcNow.AddDays(-2),
                CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(20)),
                CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(25)),
                UnitNumber = roomForBooking2.UnitNumber,
                IsPaymentSuccessful = false,
            }
        };
        await context.Bookings.AddRangeAsync(bookings);
        await context.SaveChangesAsync();
    }
}
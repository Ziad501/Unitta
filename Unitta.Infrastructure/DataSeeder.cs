using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Unitta.Application.Utility;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Identity;
using Unitta.Infrastructure.Persistence;

namespace Unitta.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        await SeedAdminUserAsync(userManager, roleManager);

        // If units already exist, we assume all seed data is present.
        if (await context.Units.AnyAsync())
        {
            return;
        }

        var faker = new Faker();

        // --- 1. SEED UNITS AND UNIT NUMBERS (No changes here) ---
        var units = new List<Unit>();
        var roomTypes = new[] { "Luxury Suite", "Deluxe King", "Standard Queen", "Executive Studio" };

        for (int i = 1; i <= 20; i++)
        {
            var roomType = faker.PickRandom(roomTypes);
            units.Add(new Unit
            {
                Name = $"{roomType} - Room {i}",
                Description = faker.Lorem.Paragraph(3),
                Price = Math.Round(faker.Random.Decimal(150, 500), 2),
                Sqft = faker.Random.Int(400, 1200),
                Occupancy = faker.Random.Int(2, 6),
                ImageUrl = $"https://picsum.photos/seed/{i}/600/400",
                CreatedDate = DateTimeOffset.UtcNow,
                Features = new List<Feature>
                {
                    new() { Name = "Free Wi-Fi" },
                    new() { Name = "Air Conditioning" },
                    new() { Name = faker.PickRandom("Pool View", "City View", "Garden View") }
                }
            });
        }
        await context.Units.AddRangeAsync(units);
        await context.SaveChangesAsync();

        var unitNumbers = new List<UnitNo>();
        var seededUnits = await context.Units.ToListAsync();
        foreach (var unit in seededUnits)
        {
            unitNumbers.Add(new UnitNo
            {
                UnitId = unit.Id,
                UnitNumber = 100 + unit.Id,
                SpecialDetails = faker.PickRandom(null, "Corner room", "Recently renovated.", "Balcony available")
            });
        }
        await context.UnitNumbers.AddRangeAsync(unitNumbers);
        await context.SaveChangesAsync();


        // --- 2. SEED USERS AND BOOKINGS FOR CHART TESTING ---

        // Define date ranges for the current and previous months
        var now = DateTimeOffset.UtcNow; // August 23, 2025
        var currentMonthStart = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
        var previousMonthStart = currentMonthStart.AddMonths(-1);

        // Seed some users in the previous month (July 2025)
        for (int i = 0; i < 5; i++)
        {
            var user = new ApplicationUser
            {
                UserName = faker.Internet.Email(),
                Email = faker.Internet.Email(),
                Name = faker.Name.FullName(),
                EmailConfirmed = true,
                CreatedAt = faker.Date.BetweenOffset(previousMonthStart, currentMonthStart.AddSeconds(-1))
            };
            await userManager.CreateAsync(user, "Password123!");
        }

        // Seed some users in the current month (August 2025)
        for (int i = 0; i < 3; i++)
        {
            var user = new ApplicationUser
            {
                UserName = faker.Internet.Email(),
                Email = faker.Internet.Email(),
                Name = faker.Name.FullName(),
                EmailConfirmed = true,
                CreatedAt = faker.Date.BetweenOffset(currentMonthStart, now)
            };
            await userManager.CreateAsync(user, "Password123!");
        }

        // Get all users and units to create bookings for
        var allUsersForBookings = await context.Users.ToListAsync();
        var allUnitsForBookings = await context.Units.Include(u => u.UnitNo).ToListAsync();
        var bookings = new List<Booking>();

        // Seed some bookings in the previous month (July 2025)
        for (int i = 0; i < 15; i++)
        {
            var randomUser = faker.PickRandom(allUsersForBookings);
            var randomUnit = faker.PickRandom(allUnitsForBookings);
            var bookingDate = faker.Date.BetweenOffset(previousMonthStart, currentMonthStart.AddSeconds(-1));
            var nights = faker.Random.Int(1, 7);

            bookings.Add(new Booking
            {
                UserId = randomUser.Id,
                UnitId = randomUnit.Id,
                Name = randomUser.Name,
                Email = randomUser.Email,
                Phone = faker.Phone.PhoneNumber(),
                TotalCost = randomUnit.Price * nights,
                Nights = nights,
                Status = faker.PickRandom(new[] { SD.StatusApproved, SD.StatusCompleted }),
                BookingDate = bookingDate,
                CheckInDate = DateOnly.FromDateTime(bookingDate.DateTime.AddDays(faker.Random.Int(5, 20))),
                CheckOutDate = DateOnly.FromDateTime(bookingDate.DateTime.AddDays(faker.Random.Int(21, 40))),
                UnitNumber = randomUnit.UnitNo!.UnitNumber,
                IsPaymentSuccessful = true,
                PaymentDate = bookingDate,
            });
        }

        // Seed some bookings in the current month (August 2025)
        for (int i = 0; i < 8; i++)
        {
            var randomUser = faker.PickRandom(allUsersForBookings);
            var randomUnit = faker.PickRandom(allUnitsForBookings);
            var bookingDate = faker.Date.BetweenOffset(currentMonthStart, now);
            var nights = faker.Random.Int(1, 7);

            bookings.Add(new Booking
            {
                UserId = randomUser.Id,
                UnitId = randomUnit.Id,
                Name = randomUser.Name,
                Email = randomUser.Email,
                Phone = faker.Phone.PhoneNumber(),
                TotalCost = randomUnit.Price * nights,
                Nights = nights,
                Status = faker.PickRandom(new[] { SD.StatusApproved, SD.StatusPending }),
                BookingDate = bookingDate,
                CheckInDate = DateOnly.FromDateTime(bookingDate.DateTime.AddDays(faker.Random.Int(5, 20))),
                CheckOutDate = DateOnly.FromDateTime(bookingDate.DateTime.AddDays(faker.Random.Int(21, 40))),
                UnitNumber = randomUnit.UnitNo!.UnitNumber,
                IsPaymentSuccessful = true,
                PaymentDate = bookingDate,
            });
        }

        await context.Bookings.AddRangeAsync(bookings);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string adminEmail = "admin@unitta.com";
        string adminPassword = "123456Z!";

        // Ensure the Admin role exists
        if (!await roleManager.RoleExistsAsync(SD.Role_Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
        }

        // Check if the admin user already exists
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            // Create the admin user
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Administrator",
                EmailConfirmed = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                // Assign the 'Admin' role to the new user
                await userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
            }
            else
            {
                // Handle potential errors during user creation
                throw new Exception($"Could not create admin user. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
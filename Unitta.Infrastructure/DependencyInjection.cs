using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unitta.Application.Interfaces;
using Unitta.Infrastructure.Email;
using Unitta.Infrastructure.Identity;
using Unitta.Infrastructure.Persistence;
using Unitta.Infrastructure.Repositories;
using Unitta.Infrastructure.Services;

namespace Unitta.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = false;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserService, IdentityUserService>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IUnitNoRepository, UnitNoRepository>();
        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }
}
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Unitta.Application.Services;

namespace Unitta.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(BookingService).Assembly);
        services.AddScoped<BookingService>();
        return services;
    }
}

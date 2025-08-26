using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Unitta.Application.Services;
using Unitta.Application.Validators;

namespace Unitta.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(RegisterDtoValidator).Assembly);
        services.AddFluentValidationClientsideAdapters();

        services.AddScoped<BookingService>();
        return services;
    }
}

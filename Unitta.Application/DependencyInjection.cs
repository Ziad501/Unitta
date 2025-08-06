using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Unitta.Application.Services;
using Unitta.Application.Validators;

namespace Unitta.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<UnitNoCreateDtoValidator>();

        services.AddScoped<BookingService>();
        return services;
    }
}

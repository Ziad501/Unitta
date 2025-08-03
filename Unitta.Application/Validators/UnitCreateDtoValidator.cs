using FluentValidation;
using Unitta.Application.DTOs;

namespace Unitta.Application.Validators;

public class UnitCreateDtoValidator : AbstractValidator<UnitCreateDto>
{
    public UnitCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Unit name is required.")
            .MaximumLength(100).WithMessage("Unit name cannot be more than 100 characters.");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be a positive number.");

        RuleFor(x => x.Sqft)
            .NotEmpty().WithMessage("Area is required.")
            .GreaterThan(0).WithMessage("Square footage must be a positive number.");

        RuleFor(x => x.Occupancy)
            .NotEmpty().WithMessage("Occupancy is required.")
            .InclusiveBetween(1, 20).WithMessage("Occupancy must be between 1 and 20.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            .Must(BeAValidHttpUrl)
            .WithMessage("A valid HTTP/HTTPS URL is required for the image. Example: https://example.com/image.jpg");
    }

    private bool BeAValidHttpUrl(string? url)
    {
        // Check if it's a valid URI and has http or https scheme
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri? validatedUri))
        {
            return validatedUri.Scheme == Uri.UriSchemeHttp || validatedUri.Scheme == Uri.UriSchemeHttps;
        }
        return false;
    }
}
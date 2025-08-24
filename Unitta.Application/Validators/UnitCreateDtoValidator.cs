using FluentValidation;
using Unitta.Application.DTOs;

namespace Unitta.Application.Validators;

public class UnitCreateDtoValidator : AbstractValidator<UnitCreateDto>
{
    public UnitCreateDtoValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Unit name is required.")
            .MaximumLength(100).WithMessage("Unit name cannot exceed 100 characters.");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Price must be a positive number.");

        RuleFor(p => p.Sqft)
            .GreaterThan(0).WithMessage("Square footage must be greater than 0.");

        RuleFor(p => p.Occupancy)
            .GreaterThanOrEqualTo(1).WithMessage("Occupancy must be at least 1.");

        RuleFor(p => p.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(p => p.Image)
            .NotNull()
            .WithMessage("An image is required.")
            .IsValidImage();
    }
}
using FluentValidation;
using Unitta.Application.DTOs;

namespace Unitta.Application.Validators;

public class FeatureUpdateDtoValidator : AbstractValidator<FeatureUpdateDto>
{
    public FeatureUpdateDtoValidator()
    {
        RuleFor(f => f.Id)
            .NotEmpty().WithMessage("Feature ID is required.")
            .GreaterThan(0).WithMessage("Invalid Feature ID.");

        RuleFor(f => f.Name)
            .NotEmpty().WithMessage("Feature name is required.")
            .MaximumLength(100).WithMessage("Feature name cannot exceed 100 characters.");

        RuleFor(f => f.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(f => f.UnitId)
            .NotEmpty().WithMessage("A unit must be selected.")
            .GreaterThan(0).WithMessage("Invalid Unit ID.");
    }
}
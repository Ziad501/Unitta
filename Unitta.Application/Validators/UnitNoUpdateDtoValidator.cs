using FluentValidation;
using Unitta.Application.DTOs;

namespace Unitta.Application.Validators;

public class UnitNoUpdateDtoValidator : AbstractValidator<UnitNoUpdateDto>
{
    public UnitNoUpdateDtoValidator()
    {
        RuleFor(x => x.UnitNumber)
            .NotEmpty()
            .WithMessage("Unit number is required.")
            .GreaterThan(0)
            .WithMessage("Unit number must be greater than zero.")
            .WithName("Unit number");
        RuleFor(x => x.UnitId)
            .NotEmpty()
            .WithMessage("Unit ID is required.")
            .GreaterThan(0)
            .WithMessage("Unit ID must be greater than zero.");
        RuleFor(x => x.SpecialDetails)
            .MaximumLength(500)
            .WithMessage("Special details cannot exceed 500 characters.")
            .WithName("Special details");
    }
}

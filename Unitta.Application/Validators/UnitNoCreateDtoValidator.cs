using FluentValidation;
using Unitta.Application.DTOs;
using Unitta.Application.Interfaces;
namespace Unitta.Application.Validators;

public class UnitNoCreateDtoValidator : AbstractValidator<UnitNoCreateDto>
{
    private readonly IUnitNoRepository _unitNoRepo;

    public UnitNoCreateDtoValidator(IUnitNoRepository unitNoRepo)
    {
        _unitNoRepo = unitNoRepo;

        RuleFor(x => x.UnitNumber)
            .NotEmpty().WithMessage("Unit number is required.")
            .MustAsync(BeAUniqueNumber)
            .WithMessage("This Unit Number already exists. Please choose another.")
            .WithName("Unit number");

        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("You must select a Unit.")
            .GreaterThan(0).WithMessage("Invalid Unit ID.")
            .MustAsync(BeAnUnassignedUnit)
            .WithMessage("The selected Unit has already been assigned a number.");

        RuleFor(x => x.SpecialDetails)
            .MaximumLength(500)
            .WithMessage("Special details cannot exceed 500 characters.")
            .WithName("Special details");
    }

    private async Task<bool> BeAnUnassignedUnit(int unitId, CancellationToken cancellationToken)
    {
        return !await _unitNoRepo.AnyAsync(un => un.UnitId == unitId);
    }

    private async Task<bool> BeAUniqueNumber(int unitNumber, CancellationToken cancellationToken)
    {
        return !await _unitNoRepo.AnyAsync(un => un.UnitNumber == unitNumber);
    }
}
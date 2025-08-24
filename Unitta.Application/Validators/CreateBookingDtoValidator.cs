using FluentValidation;
using Unitta.Application.DTOs;
using Unitta.Application.Interfaces;

namespace Unitta.Application.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    private readonly IBookingRepository _bookingRepository;
    public CreateBookingDtoValidator(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;

        RuleFor(b => b.UnitId)
            .NotEmpty().WithMessage("Unit ID is required.")
            .GreaterThan(0).WithMessage("Unit ID must be a positive number.");

        RuleFor(b => b.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

        RuleFor(b => b.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(b => b.Nights)
            .NotEmpty().WithMessage("Number of nights is required.")
            .GreaterThan(0).WithMessage("You must book for at least one night.");

        RuleFor(b => b.CheckInDate)
            .NotEmpty().WithMessage("Check-in date is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Check-in date cannot be in the past.");

        RuleFor(b => b.UnitNumber)
            .NotEmpty().WithMessage("Unit number is required.")
            .GreaterThan(0).WithMessage("Unit number must be a positive number.");
        RuleFor(booking => booking)
            .MustAsync(BeAvailable)
            .WithMessage("This unit is not available for the selected dates. Please choose different dates.");
    }
    private async Task<bool> BeAvailable(CreateBookingDto booking, CancellationToken cancellationToken)
    {
        if (booking.UnitId <= 0 || booking.Nights <= 0)
        {
            return true;
        }

        var checkOutDate = booking.CheckInDate.AddDays(booking.Nights);

        var conflictingBookings = await _bookingRepository.GetConflictingBookingsAsync(
            booking.UnitId,
            booking.CheckInDate,
            checkOutDate);

        return !conflictingBookings.Any();
    }
}
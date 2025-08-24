using FluentValidation;
using Unitta.Application.DTOs;

namespace Unitta.Application.Validators
{
    public class UpdateBookingDtoValidator : AbstractValidator<UpdateBookingDto>
    {
        public UpdateBookingDtoValidator()
        {
            RuleFor(b => b.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

            RuleFor(b => b.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(b => b.Status)
                .NotEmpty().WithMessage("Booking status is required.")
                .MaximumLength(50).WithMessage("Status cannot exceed 50 characters.");
        }
    }
}
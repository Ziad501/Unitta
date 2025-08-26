using FluentValidation;
using Unitta.Application.DTOs;

namespace Unitta.Application.Validators;

internal class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Password reset code is required.")
            .WithName("Reset Code");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("New password is required.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Please confirm your new password.")
            .Equal(x => x.Password)
            .WithMessage("The password and confirmation password do not match.")
            .WithName("Confirm password");
    }
}
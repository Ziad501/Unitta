using FluentValidation;
using Unitta.Application.DTOs;
using Unitta.Application.Interfaces;

namespace Unitta.Application.Validators;

public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    private readonly IUserService _userService;

    public ForgotPasswordDtoValidator(IUserService userService)
    {
        _userService = userService;
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.").MustAsync(async (email, cancellationToken) => await UserExists(email))
            .WithMessage("No account was found with that email address.");
    }

    private async Task<bool> UserExists(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        return user != null;
    }
}
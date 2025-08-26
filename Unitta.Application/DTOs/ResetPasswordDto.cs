namespace Unitta.Application.DTOs;

public class ResetPasswordDto
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

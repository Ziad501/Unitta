using Microsoft.AspNetCore.Identity;

namespace Unitta.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public string? PasswordResetCodeHash { get; set; }
        public DateTime? PasswordResetCodeSentAt { get; set; }
    }
}

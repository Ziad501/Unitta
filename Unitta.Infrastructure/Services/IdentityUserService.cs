using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Unitta.Application.Interfaces;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Identity;


namespace Unitta.Infrastructure.Services;

public class IdentityUserService(
    UserManager<ApplicationUser> _userManager,
    SignInManager<ApplicationUser> _signInManager) : IUserService
{
    public async Task<User?> GetUserByIdAsync(string userId)
    {
        var identityUser = await _userManager.FindByIdAsync(userId);
        return identityUser == null ? null : MapToDomainUser(identityUser);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);
        return identityUser == null ? null : MapToDomainUser(identityUser);
    }

    public async Task<bool> CreateUserAsync(string email, string name, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task<bool> ValidateUserAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        return result.Succeeded;
    }

    public async Task<string?> GetUserIdByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user?.Id;
    }

    private static User MapToDomainUser(ApplicationUser identityUser)
    {
        return new User
        {
            Id = identityUser.Id,
            Name = identityUser.Name,
            Email = identityUser.Email ?? string.Empty,
            Phone = identityUser.PhoneNumber ?? string.Empty,
            CreatedAt = identityUser.CreatedAt
        };
    }
    public async Task<IEnumerable<User>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null)
    {
        var identityUsers = _userManager.Users;
        var users = new List<User>();
        await foreach (var identityUser in identityUsers.AsAsyncEnumerable())
        {
            users.Add(MapToDomainUser(identityUser));
        }

        return filter == null ? users : users.AsQueryable().Where(filter).ToList();
    }

}
using System.Linq.Expressions;
using Unitta.Domain.Entities;

namespace Unitta.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(string userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> CreateUserAsync(string email, string name, string password);
    Task<bool> ValidateUserAsync(string email, string password);
    Task<string?> GetUserIdByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null);
}

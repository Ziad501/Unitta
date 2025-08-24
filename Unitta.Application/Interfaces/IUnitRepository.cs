using System.Linq.Expressions;
using Unitta.Domain.Entities;

namespace Unitta.Application.Interfaces;

public interface IUnitRepository
{
    Task<Unit?> GetByIdAsync(int id);
    Task<IEnumerable<Unit>> GetAllAsync();
    Task<IEnumerable<Unit>> GetAllAsync(Expression<Func<Unit, bool>> predicate);
    Task<IEnumerable<Unit>> GetAvailableUnitsAsync(DateOnly checkIn, DateOnly checkOut);
    Task CreateAsync(Unit unit);
    Task UpdateAsync(Unit unit);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
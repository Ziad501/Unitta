using Unitta.Domain.Entities;

namespace Unitta.Application.Interfaces;

public interface IUnitRepository
{
    Task<Unit?> GetByIdAsync(int id);
    Task<IEnumerable<Unit>> GetAllAsync();
    Task<IEnumerable<Unit>> GetAvailableUnitsAsync(DateOnly checkIn, DateOnly checkOut);
    Task<bool> CreateAsync(Unit unit);
    Task<bool> UpdateAsync(Unit unit);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
using System.Linq.Expressions;
using Unitta.Domain.Entities;

namespace Unitta.Application.Interfaces;

public interface IUnitNoRepository
{
    Task<UnitNo?> GetByUnitNumberAsync(int unitNumber);
    Task<IEnumerable<UnitNo>> GetAllAsync();
    Task<UnitNo> GetByUnitIdAsync(int unitId);
    Task CreateAsync(UnitNo unitNo);
    Task UpdateAsync(UnitNo unitNo);
    Task DeleteAsync(UnitNo unitNo);
    Task<bool> AnyAsync(Expression<Func<UnitNo, bool>> predicate);
}
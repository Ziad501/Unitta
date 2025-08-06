using System.Linq.Expressions;
using Unitta.Domain.Entities;

namespace Unitta.Application.Interfaces;

public interface IUnitNoRepository
{
    Task<UnitNo?> GetByUnitNumberAsync(int unitNumber);
    Task<IEnumerable<UnitNo>> GetAllAsync();
    Task<IEnumerable<UnitNo>> GetByUnitIdAsync(int unitId);
    Task<bool> CreateAsync(UnitNo unitNo);
    Task<bool> UpdateAsync(UnitNo unitNo);
    Task<bool> DeleteAsync(int unitNumber);
    Task<bool> AnyAsync(Expression<Func<UnitNo, bool>> predicate);

}
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Unitta.Application.Interfaces;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Persistence;

namespace Unitta.Infrastructure.Repositories;

public class UnitNoRepository(ApplicationDbContext _context) : IUnitNoRepository
{
    public async Task<UnitNo?> GetByUnitNumberAsync(int unitNumber)
    {
        return await _context.UnitNumbers
            .Include(un => un.Unit)
            .FirstOrDefaultAsync(un => un.UnitNumber == unitNumber);
    }

    public async Task<IEnumerable<UnitNo>> GetAllAsync()
    {
        return await _context.UnitNumbers
            .AsNoTracking()
            .Include(un => un.Unit)
            .OrderBy(un => un.UnitNumber)
            .ToListAsync();
    }

    public async Task<UnitNo> GetByUnitIdAsync(int unitId)
    {
        return await _context.UnitNumbers
            .AsNoTracking()
            .Include(un => un.Unit)
            .FirstOrDefaultAsync(un => un.UnitId == unitId);
    }

    public async Task CreateAsync(UnitNo unitNo)
    {
        _context.UnitNumbers.Add(unitNo);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UnitNo unitNo)
    {
        _context.UnitNumbers.Update(unitNo);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(UnitNo unitNo)
    {
        _context.UnitNumbers.Remove(unitNo);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> AnyAsync(Expression<Func<UnitNo, bool>> predicate)
    {
        return await _context.UnitNumbers.AnyAsync(predicate);
    }
}
using Microsoft.EntityFrameworkCore;
using Unitta.Application.Interfaces;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Persistence;

namespace Unitta.Infrastructure.Repositories;

public class UnitNoRepository : IUnitNoRepository
{
    private readonly ApplicationDbContext _context;

    public UnitNoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UnitNo?> GetByUnitNumberAsync(int unitNumber)
    {
        return await _context.UnitNumbers
            .Include(un => un.Unit)
            .FirstOrDefaultAsync(un => un.UnitNumber == unitNumber);
    }

    public async Task<IEnumerable<UnitNo>> GetAllAsync()
    {
        return await _context.UnitNumbers
            .Include(un => un.Unit)
            .OrderBy(un => un.UnitNumber)
            .ToListAsync();
    }

    public async Task<IEnumerable<UnitNo>> GetByUnitIdAsync(int unitId)
    {
        return await _context.UnitNumbers
            .Where(un => un.UnitId == unitId)
            .OrderBy(un => un.UnitNumber)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(UnitNo unitNo)
    {
        try
        {
            _context.UnitNumbers.Add(unitNo);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(UnitNo unitNo)
    {
        try
        {
            _context.UnitNumbers.Update(unitNo);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int unitNumber)
    {
        try
        {
            var unitNo = await _context.UnitNumbers.FindAsync(unitNumber);
            if (unitNo == null) return false;

            _context.UnitNumbers.Remove(unitNo);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }
}
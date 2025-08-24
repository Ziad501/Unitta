using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Unitta.Application.Interfaces;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Persistence;

namespace Unitta.Infrastructure.Repositories;

public class UnitRepository(ApplicationDbContext _context) : IUnitRepository
{
    public async Task<Unit?> GetByIdAsync(int id)
    {
        return await _context.Units
            .Include(u => u.Features)
            .Include(u => u.UnitNo)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<Unit>> GetAllAsync()
    {
        return await _context.Units
            .AsNoTracking()
            .Include(u => u.Features)
            .Include(u => u.UnitNo)
            .OrderBy(u => u.Id)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Unit>> GetAvailableUnitsAsync(DateOnly checkIn, DateOnly checkOut)
    {
        var bookedUnitIds = await _context.Bookings
            .AsNoTracking()
            .Where(b => b.CheckInDate < checkOut && b.CheckOutDate > checkIn)
            .Select(b => b.UnitId)
            .Distinct()
            .ToListAsync();

        return await _context.Units
            .Include(u => u.Features)
            .Include(u => u.UnitNo)
            .Where(u => !bookedUnitIds.Contains(u.Id))
            .OrderBy(u => u.Name)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task CreateAsync(Unit unit)
    {
        _context.Units.Add(unit);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Unit unit)
    {
        var existingUnit = await _context.Units.FindAsync(unit.Id);
        if (existingUnit != null)
        {
            _context.Entry(existingUnit).CurrentValues.SetValues(unit);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var unit = await _context.Units.FindAsync(id);
        if (unit == null) return;

        _context.Units.Remove(unit);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Units.AnyAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<Unit>> GetAllAsync(Expression<Func<Unit, bool>> predicate)
    {
        return await _context.Units
            .Include(u => u.Features)
            .Include(u => u.UnitNo)
            .OrderBy(u => u.Id)
            .Where(predicate)
            .AsSplitQuery()
            .ToListAsync();
    }
}
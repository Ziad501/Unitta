using Microsoft.EntityFrameworkCore;
using Unitta.Application.Interfaces;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Persistence;

namespace Unitta.Infrastructure.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly ApplicationDbContext _context;

    public UnitRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit?> GetByIdAsync(int id)
    {
        return await _context.Units
            .Include(u => u.Features)
            .Include(u => u.UnitNumbers)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<Unit>> GetAllAsync()
    {
        return await _context.Units
            .Include(u => u.Features)
            .Include(u => u.UnitNumbers)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Unit>> GetAvailableUnitsAsync(DateOnly checkIn, DateOnly checkOut)
    {
        var bookedUnitIds = await _context.Bookings
            .Where(b => b.CheckInDate < checkOut && b.CheckOutDate > checkIn)
            .Select(b => b.UnitId)
            .Distinct()
            .ToListAsync();

        return await _context.Units
            .Include(u => u.Features)
            .Include(u => u.UnitNumbers)
            .Where(u => !bookedUnitIds.Contains(u.Id))
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(Unit unit)
    {
        try
        {
            unit.CreatedDate = DateTime.UtcNow;
            _context.Units.Add(unit);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Unit unit)
    {
        try
        {
            unit.UpdatedDate = DateTime.UtcNow;
            _context.Units.Update(unit);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null) return false;

            _context.Units.Remove(unit);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Units.AnyAsync(u => u.Id == id);
    }
}
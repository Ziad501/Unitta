using Microsoft.EntityFrameworkCore;
using Unitta.Application.Interfaces;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Persistence;

namespace Unitta.Infrastructure.Repositories;

public class FeatureRepository(ApplicationDbContext _context) : IFeatureRepository
{
    public async Task<Feature?> GetByIdAsync(int id)
    {
        return await _context.Features
            .Include(f => f.Unit)
            .AsSplitQuery()
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Feature>> GetAllAsync()
    {
        return await _context.Features
            .AsNoTracking()
            .Include(f => f.Unit)
            .OrderBy(f => f.Name)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Feature>> GetByUnitIdAsync(int unitId)
    {
        return await _context.Features
            .AsNoTracking()
            .Where(f => f.UnitId == unitId)
            .OrderBy(f => f.Name)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task CreateAsync(Feature feature)
    {
        _context.Features.Add(feature);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Feature feature)
    {
        _context.Features.Update(feature);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var feature = await _context.Features.FindAsync(id);
        if (feature == null) return;
        _context.Features.Remove(feature);
        await _context.SaveChangesAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using Unitta.Application.Interfaces;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Persistence;

namespace Unitta.Infrastructure.Repositories;

public class FeatureRepository : IFeatureRepository
{
    private readonly ApplicationDbContext _context;

    public FeatureRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Feature?> GetByIdAsync(int id)
    {
        return await _context.Features
            .Include(f => f.Unit)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Feature>> GetAllAsync()
    {
        return await _context.Features
            .Include(f => f.Unit)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Feature>> GetByUnitIdAsync(int unitId)
    {
        return await _context.Features
            .Where(f => f.UnitId == unitId)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(Feature feature)
    {
        try
        {
            _context.Features.Add(feature);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Feature feature)
    {
        try
        {
            _context.Features.Update(feature);
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
            var feature = await _context.Features.FindAsync(id);
            if (feature == null) return false;

            _context.Features.Remove(feature);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }
}
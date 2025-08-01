using Unitta.Domain.Entities;

namespace Unitta.Application.Interfaces;

public interface IFeatureRepository
{
    Task<Feature?> GetByIdAsync(int id);
    Task<IEnumerable<Feature>> GetAllAsync();
    Task<IEnumerable<Feature>> GetByUnitIdAsync(int unitId);
    Task<bool> CreateAsync(Feature feature);
    Task<bool> UpdateAsync(Feature feature);
    Task<bool> DeleteAsync(int id);
}
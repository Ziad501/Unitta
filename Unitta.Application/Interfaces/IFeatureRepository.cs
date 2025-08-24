using Unitta.Domain.Entities;

namespace Unitta.Application.Interfaces;

public interface IFeatureRepository
{
    Task<Feature?> GetByIdAsync(int id);
    Task<IEnumerable<Feature>> GetAllAsync();
    Task<IEnumerable<Feature>> GetByUnitIdAsync(int unitId);
    Task CreateAsync(Feature feature);
    Task UpdateAsync(Feature feature);
    Task DeleteAsync(int id);
}
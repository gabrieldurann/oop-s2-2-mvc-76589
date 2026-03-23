using FoodSafetyTracker.Domain.Entities;

namespace FoodSafetyTracker.Domain.Interfaces;

public interface IInspectionRepository
{
    Task<IEnumerable<Inspection>> GetAllAsync();
    Task<Inspection?> GetByIdAsync(int id);
    Task<IEnumerable<Inspection>> GetByPremisesIdAsync(int premisesId);
    Task AddAsync(Inspection inspection);
    Task UpdateAsync(Inspection inspection);
    Task DeleteAsync(int id);
}
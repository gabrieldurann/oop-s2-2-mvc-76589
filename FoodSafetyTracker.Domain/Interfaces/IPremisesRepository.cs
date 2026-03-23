using FoodSafetyTracker.Domain.Entities;

namespace FoodSafetyTracker.Domain.Interfaces;

public interface IPremisesRepository
{
    Task<IEnumerable<Premises>> GetAllAsync();
    Task<Premises?> GetByIdAsync(int id);
    Task AddAsync(Premises premises);
    Task UpdateAsync(Premises premises);
    Task DeleteAsync(int id);
}
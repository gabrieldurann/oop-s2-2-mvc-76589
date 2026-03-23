using FoodSafetyTracker.Domain.Entities;

namespace FoodSafetyTracker.Domain.Interfaces;

public interface IFollowUpRepository
{
    Task<IEnumerable<FollowUp>> GetAllAsync();
    Task<FollowUp?> GetByIdAsync(int id);
    Task<IEnumerable<FollowUp>> GetByInspectionIdAsync(int inspectionId);
    Task<IEnumerable<FollowUp>> GetOverdueAsync();
    Task AddAsync(FollowUp followUp);
    Task UpdateAsync(FollowUp followUp);
    Task DeleteAsync(int id);
}
using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Entities.Enums;
using FoodSafetyTracker.Domain.Interfaces;
using FoodSafetyTracker.MVC.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.MVC.Repositories;

public class FollowUpRepository : IFollowUpRepository
{
    private readonly ApplicationDbContext _context;

    public FollowUpRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FollowUp>> GetAllAsync() =>
        await _context.FollowUps
            .Include(f => f.Inspection)
            .ToListAsync();

    public async Task<FollowUp?> GetByIdAsync(int id) =>
        await _context.FollowUps
            .Include(f => f.Inspection)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<IEnumerable<FollowUp>> GetByInspectionIdAsync(int inspectionId) =>
        await _context.FollowUps
            .Where(f => f.InspectionId == inspectionId)
            .ToListAsync();

    public async Task<IEnumerable<FollowUp>> GetOverdueAsync() =>
        await _context.FollowUps
            .Where(f => f.Status == FollowUpStatus.Open && f.DueDate < DateTime.Today)
            .Include(f => f.Inspection)
            .ToListAsync();

    public async Task AddAsync(FollowUp followUp)
    {
        _context.FollowUps.Add(followUp);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FollowUp followUp)
    {
        _context.FollowUps.Update(followUp);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var followUp = await _context.FollowUps.FindAsync(id);
        if (followUp != null)
        {
            _context.FollowUps.Remove(followUp);
            await _context.SaveChangesAsync();
        }
    }
}
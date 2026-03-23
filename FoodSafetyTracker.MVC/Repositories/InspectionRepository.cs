using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Interfaces;
using FoodSafetyTracker.MVC.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.MVC.Repositories;

public class InspectionRepository : IInspectionRepository
{
    private readonly ApplicationDbContext _context;

    public InspectionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Inspection>> GetAllAsync() =>
        await _context.Inspections
            .Include(i => i.Premises)
            .ToListAsync();

    public async Task<Inspection?> GetByIdAsync(int id) =>
        await _context.Inspections
            .Include(i => i.Premises)
            .Include(i => i.FollowUps)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Inspection>> GetByPremisesIdAsync(int premisesId) =>
        await _context.Inspections
            .Where(i => i.PremisesId == premisesId)
            .Include(i => i.Premises)
            .ToListAsync();

    public async Task AddAsync(Inspection inspection)
    {
        _context.Inspections.Add(inspection);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Inspection inspection)
    {
        _context.Inspections.Update(inspection);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var inspection = await _context.Inspections.FindAsync(id);
        if (inspection != null)
        {
            _context.Inspections.Remove(inspection);
            await _context.SaveChangesAsync();
        }
    }
}
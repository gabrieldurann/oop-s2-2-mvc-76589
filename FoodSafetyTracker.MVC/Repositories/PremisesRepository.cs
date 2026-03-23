using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Interfaces;
using FoodSafetyTracker.MVC.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.MVC.Repositories;

public class PremisesRepository : IPremisesRepository
{
    private readonly ApplicationDbContext _context;

    public PremisesRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Premises>> GetAllAsync() =>
        await _context.Premises.ToListAsync();

    public async Task<Premises?> GetByIdAsync(int id) =>
        await _context.Premises
            .Include(p => p.Inspections)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Premises premises)
    {
        _context.Premises.Add(premises);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Premises premises)
    {
        _context.Premises.Update(premises);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var premises = await _context.Premises.FindAsync(id);
        if (premises != null)
        {
            _context.Premises.Remove(premises);
            await _context.SaveChangesAsync();
        }
    }
}
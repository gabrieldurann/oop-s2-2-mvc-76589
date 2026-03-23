using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Entities.Enums;
using FoodSafetyTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafetyTracker.MVC.Controllers;

[Authorize]
public class PremisesController : Controller
{
    private readonly IPremisesRepository _premisesRepository;
    private readonly ILogger<PremisesController> _logger;

    public PremisesController(IPremisesRepository premisesRepository, ILogger<PremisesController> logger)
    {
        _premisesRepository = premisesRepository;
        _logger = logger;
    }

    // GET: Premises
    public async Task<IActionResult> Index()
    {
        var premises = await _premisesRepository.GetAllAsync();
        return View(premises);
    }

    // GET: Premises/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var premises = await _premisesRepository.GetByIdAsync(id);
        if (premises == null)
        {
            _logger.LogWarning("Premises with ID {PremisesId} not found", id);
            return NotFound();
        }
        return View(premises);
    }

    // GET: Premises/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Premises/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Premises premises)
    {
        if (!ModelState.IsValid) return View(premises);

        await _premisesRepository.AddAsync(premises);
        _logger.LogInformation("Premises created: {PremisesName} in {Town} with ID {PremisesId}",
            premises.Name, premises.Town, premises.Id);
        return RedirectToAction(nameof(Index));
    }

    // GET: Premises/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var premises = await _premisesRepository.GetByIdAsync(id);
        if (premises == null)
        {
            _logger.LogWarning("Premises with ID {PremisesId} not found for edit", id);
            return NotFound();
        }
        return View(premises);
    }

    // POST: Premises/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Premises premises)
    {
        if (id != premises.Id) return BadRequest();
        if (!ModelState.IsValid) return View(premises);

        await _premisesRepository.UpdateAsync(premises);
        _logger.LogInformation("Premises updated: ID {PremisesId}", premises.Id);
        return RedirectToAction(nameof(Index));
    }

    // GET: Premises/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var premises = await _premisesRepository.GetByIdAsync(id);
        if (premises == null)
        {
            _logger.LogWarning("Premises with ID {PremisesId} not found for deletion", id);
            return NotFound();
        }
        return View(premises);
    }

    // POST: Premises/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _premisesRepository.DeleteAsync(id);
        _logger.LogInformation("Premises deleted: ID {PremisesId}", id);
        return RedirectToAction(nameof(Index));
    }
}
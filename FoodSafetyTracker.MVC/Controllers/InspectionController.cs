using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafetyTracker.MVC.Controllers;

[Authorize]
public class InspectionController : Controller
{
    private readonly IInspectionRepository _inspectionRepository;
    private readonly IPremisesRepository _premisesRepository;
    private readonly ILogger<InspectionController> _logger;

    public InspectionController(
        IInspectionRepository inspectionRepository,
        IPremisesRepository premisesRepository,
        ILogger<InspectionController> logger)
    {
        _inspectionRepository = inspectionRepository;
        _premisesRepository = premisesRepository;
        _logger = logger;
    }

    // GET: Inspection
    public async Task<IActionResult> Index()
    {
        var inspections = await _inspectionRepository.GetAllAsync();
        return View(inspections);
    }

    // GET: Inspection/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var inspection = await _inspectionRepository.GetByIdAsync(id);
        if (inspection == null)
        {
            _logger.LogWarning("Inspection with ID {InspectionId} not found", id);
            return NotFound();
        }
        return View(inspection);
    }

    // GET: Inspection/Create
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Premises = await _premisesRepository.GetAllAsync();
        return View();
    }

    // POST: Inspection/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Create(Inspection inspection)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Premises = await _premisesRepository.GetAllAsync();
            return View(inspection);
        }

        await _inspectionRepository.AddAsync(inspection);
        _logger.LogInformation("Inspection created for PremisesId {PremisesId} with ID {InspectionId}",
            inspection.PremisesId, inspection.Id);
        return RedirectToAction(nameof(Index));
    }

    // GET: Inspection/Edit/5
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Edit(int id)
    {
        var inspection = await _inspectionRepository.GetByIdAsync(id);
        if (inspection == null)
        {
            _logger.LogWarning("Inspection with ID {InspectionId} not found for edit", id);
            return NotFound();
        }
        ViewBag.Premises = await _premisesRepository.GetAllAsync();
        return View(inspection);
    }

    // POST: Inspection/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Edit(int id, Inspection inspection)
    {
        if (id != inspection.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Premises = await _premisesRepository.GetAllAsync();
            return View(inspection);
        }

        await _inspectionRepository.UpdateAsync(inspection);
        _logger.LogInformation("Inspection updated: ID {InspectionId}", inspection.Id);
        return RedirectToAction(nameof(Index));
    }

    // GET: Inspection/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var inspection = await _inspectionRepository.GetByIdAsync(id);
        if (inspection == null)
        {
            _logger.LogWarning("Inspection with ID {InspectionId} not found for deletion", id);
            return NotFound();
        }
        return View(inspection);
    }

    // POST: Inspection/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _inspectionRepository.DeleteAsync(id);
        _logger.LogInformation("Inspection deleted: ID {InspectionId}", id);
        return RedirectToAction(nameof(Index));
    }
}
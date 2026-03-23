using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Entities.Enums;
using FoodSafetyTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafetyTracker.MVC.Controllers;

[Authorize]
public class FollowUpController : Controller
{
    private readonly IFollowUpRepository _followUpRepository;
    private readonly IInspectionRepository _inspectionRepository;
    private readonly ILogger<FollowUpController> _logger;

    public FollowUpController(
        IFollowUpRepository followUpRepository,
        IInspectionRepository inspectionRepository,
        ILogger<FollowUpController> logger)
    {
        _followUpRepository = followUpRepository;
        _inspectionRepository = inspectionRepository;
        _logger = logger;
    }

    // GET: FollowUp
    public async Task<IActionResult> Index()
    {
        var followUps = await _followUpRepository.GetAllAsync();
        return View(followUps);
    }

    // GET: FollowUp/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var followUp = await _followUpRepository.GetByIdAsync(id);
        if (followUp == null)
        {
            _logger.LogWarning("FollowUp with ID {FollowUpId} not found", id);
            return NotFound();
        }
        return View(followUp);
    }

    // GET: FollowUp/Create
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Create(int? inspectionId)
    {
        ViewBag.Inspections = await _inspectionRepository.GetAllAsync();
        if (inspectionId.HasValue)
            ViewBag.SelectedInspectionId = inspectionId.Value;
        return View();
    }

    // POST: FollowUp/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Create(FollowUp followUp)
    {
        // Business rule: DueDate cannot be before InspectionDate
        var inspection = await _inspectionRepository.GetByIdAsync(followUp.InspectionId);
        if (inspection != null && followUp.DueDate < inspection.InspectionDate)
        {
            _logger.LogWarning(
                "FollowUp DueDate {DueDate} is before InspectionDate {InspectionDate} for InspectionId {InspectionId}",
                followUp.DueDate, inspection.InspectionDate, followUp.InspectionId);
            ModelState.AddModelError("DueDate", "Due date cannot be before the inspection date.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Inspections = await _inspectionRepository.GetAllAsync();
            return View(followUp);
        }

        await _followUpRepository.AddAsync(followUp);
        _logger.LogInformation("FollowUp created for InspectionId {InspectionId} with ID {FollowUpId}",
            followUp.InspectionId, followUp.Id);
        return RedirectToAction(nameof(Index));
    }

    // GET: FollowUp/Close/5
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Close(int id)
    {
        var followUp = await _followUpRepository.GetByIdAsync(id);
        if (followUp == null)
        {
            _logger.LogWarning("FollowUp with ID {FollowUpId} not found for closing", id);
            return NotFound();
        }
        return View(followUp);
    }
    
    // POST: FollowUp/Close/5
    [HttpPost, ActionName("Close")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> CloseConfirmed(int id, DateTime? closedDate)
    {
        var followUp = await _followUpRepository.GetByIdAsync(id);
        if (followUp == null) return NotFound();

        if (!closedDate.HasValue)
        {
            _logger.LogWarning("FollowUp ID {FollowUpId} cannot be closed without a ClosedDate", id);
            ModelState.AddModelError("", "A closed date is required to close a follow-up.");
            return View(followUp);
        }

        followUp.Status = FollowUpStatus.Closed;
        followUp.ClosedDate = closedDate.Value;
        await _followUpRepository.UpdateAsync(followUp);
        _logger.LogInformation("FollowUp closed: ID {FollowUpId} on {ClosedDate}", id, closedDate.Value);
        return RedirectToAction(nameof(Index));
    }

    // GET: FollowUp/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var followUp = await _followUpRepository.GetByIdAsync(id);
        if (followUp == null)
        {
            _logger.LogWarning("FollowUp with ID {FollowUpId} not found for deletion", id);
            return NotFound();
        }
        return View(followUp);
    }

    // POST: FollowUp/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _followUpRepository.DeleteAsync(id);
        _logger.LogInformation("FollowUp deleted: ID {FollowUpId}", id);
        return RedirectToAction(nameof(Index));
    }
}
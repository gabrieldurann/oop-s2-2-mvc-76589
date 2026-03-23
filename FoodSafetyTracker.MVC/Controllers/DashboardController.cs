using FoodSafetyTracker.Domain.Entities.Enums;
using FoodSafetyTracker.MVC.Data;
using FoodSafetyTracker.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.MVC.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? town, string? riskRating)
    {
        _logger.LogInformation("Dashboard accessed by {UserName} with filters Town={Town}, RiskRating={RiskRating}",
            User.Identity?.Name, town, riskRating);

        var now = DateTime.Today;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var premisesQuery = _context.Premises.AsQueryable();

        if (!string.IsNullOrEmpty(town))
            premisesQuery = premisesQuery.Where(p => p.Town == town);

        if (!string.IsNullOrEmpty(riskRating) && Enum.TryParse<RiskRating>(riskRating, out var parsedRating))
            premisesQuery = premisesQuery.Where(p => p.RiskRating == parsedRating);

        var filteredPremisesIds = await premisesQuery.Select(p => p.Id).ToListAsync();

        var inspectionsThisMonth = await _context.Inspections
            .Where(i => filteredPremisesIds.Contains(i.PremisesId)
                        && i.InspectionDate >= startOfMonth
                        && i.InspectionDate <= now)
            .CountAsync();

        var failedInspectionsThisMonth = await _context.Inspections
            .Where(i => filteredPremisesIds.Contains(i.PremisesId)
                        && i.InspectionDate >= startOfMonth
                        && i.InspectionDate <= now
                        && i.Outcome == InspectionOutcome.Fail)
            .CountAsync();

        var overdueFollowUps = await _context.FollowUps
            .Where(f => f.Status == FollowUpStatus.Open
                        && f.DueDate < now
                        && f.Inspection != null && filteredPremisesIds.Contains(f.Inspection.PremisesId))
            .CountAsync();

        var availableTowns = await _context.Premises
            .Select(p => p.Town)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        var viewModel = new DashboardViewModel
        {
            InspectionsThisMonth = inspectionsThisMonth,
            FailedInspectionsThisMonth = failedInspectionsThisMonth,
            OverdueFollowUps = overdueFollowUps,
            SelectedTown = town,
            SelectedRiskRating = riskRating,
            AvailableTowns = availableTowns
        };

        return View(viewModel);
    }
}
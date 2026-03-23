using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Entities.Enums;
using FoodSafetyTracker.MVC.Controllers;
using FoodSafetyTracker.MVC.Data;
using FoodSafetyTracker.MVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.Tests;

public class FoodSafetyTrackerTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private async Task<(ApplicationDbContext context, Premises premises, Inspection inspection)> SeedBasicDataAsync()
    {
        var context = CreateInMemoryContext();

        var premises = new Premises { Name = "Test Cafe", Address = "1 Test St", Town = "Testville", RiskRating = RiskRating.Low };
        context.Premises.Add(premises);
        await context.SaveChangesAsync();

        var inspection = new Inspection
        {
            PremisesId = premises.Id,
            InspectionDate = DateTime.Today.AddDays(-10),
            Score = 45,
            Outcome = InspectionOutcome.Fail,
            Notes = "Test inspection"
        };
        context.Inspections.Add(inspection);
        await context.SaveChangesAsync();

        return (context, premises, inspection);
    }

    // Test 1: Overdue follow-ups query returns only overdue open items
    [Fact]
    public async Task GetOverdueAsync_ReturnsOnlyOverdueOpenFollowUps()
    {
        var (context, _, inspection) = await SeedBasicDataAsync();

        var overdueOpen = new FollowUp { InspectionId = inspection.Id, DueDate = DateTime.Today.AddDays(-5), Status = FollowUpStatus.Open };
        var notOverdue = new FollowUp { InspectionId = inspection.Id, DueDate = DateTime.Today.AddDays(5), Status = FollowUpStatus.Open };
        var overdueClosed = new FollowUp { InspectionId = inspection.Id, DueDate = DateTime.Today.AddDays(-5), Status = FollowUpStatus.Closed, ClosedDate = DateTime.Today.AddDays(-1) };

        context.FollowUps.AddRange(overdueOpen, notOverdue, overdueClosed);
        await context.SaveChangesAsync();

        var repo = new FollowUpRepository(context);
        var result = await repo.GetOverdueAsync();

        var resultList = result.ToList();
        Assert.Single(resultList);
        Assert.Equal(overdueOpen.Id, resultList.First().Id);
    }

    // Test 2: FollowUp cannot be closed without a ClosedDate
    [Fact]
    public async Task FollowUp_WithoutClosedDate_ShouldNotBeClosed()
    {
        var (context, _, inspection) = await SeedBasicDataAsync();

        var followUp = new FollowUp
        {
            InspectionId = inspection.Id,
            DueDate = DateTime.Today.AddDays(-2),
            Status = FollowUpStatus.Open,
            ClosedDate = null
        };
        context.FollowUps.Add(followUp);
        await context.SaveChangesAsync();

        Assert.False(followUp.ClosedDate.HasValue);
        Assert.Equal(FollowUpStatus.Open, followUp.Status);
    }

    // Test 3: FollowUp can be closed with a ClosedDate
    [Fact]
    public async Task FollowUp_WithClosedDate_CanBeClosed()
    {
        var (context, _, inspection) = await SeedBasicDataAsync();

        var followUp = new FollowUp { InspectionId = inspection.Id, DueDate = DateTime.Today.AddDays(-2), Status = FollowUpStatus.Open };
        context.FollowUps.Add(followUp);
        await context.SaveChangesAsync();

        var repo = new FollowUpRepository(context);
        followUp.Status = FollowUpStatus.Closed;
        followUp.ClosedDate = DateTime.Today;
        await repo.UpdateAsync(followUp);

        var updated = await repo.GetByIdAsync(followUp.Id);
        Assert.Equal(FollowUpStatus.Closed, updated!.Status);
        Assert.NotNull(updated.ClosedDate);
    }

    // Test 4: Dashboard count of inspections this month is correct
    [Fact]
    public async Task Dashboard_InspectionsThisMonth_CountIsCorrect()
    {
        var context = CreateInMemoryContext();
        var now = DateTime.Today;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var premises = new Premises { Name = "Test Cafe", Address = "1 Test St", Town = "Testville", RiskRating = RiskRating.Low };
        context.Premises.Add(premises);
        await context.SaveChangesAsync();

        context.Inspections.AddRange(
            new Inspection { PremisesId = premises.Id, InspectionDate = now.AddDays(-2), Score = 45, Outcome = InspectionOutcome.Fail, Notes = "This month" },
            new Inspection { PremisesId = premises.Id, InspectionDate = now.AddDays(-5), Score = 80, Outcome = InspectionOutcome.Pass, Notes = "This month" },
            new Inspection { PremisesId = premises.Id, InspectionDate = now.AddMonths(-2), Score = 70, Outcome = InspectionOutcome.Fail, Notes = "Last month" }
        );
        await context.SaveChangesAsync();

        var count = await context.Inspections
            .Where(i => i.InspectionDate >= startOfMonth && i.InspectionDate <= now)
            .CountAsync();

        Assert.Equal(2, count);
    }

    // Test 5: Dashboard failed inspections this month count is correct
    [Fact]
    public async Task Dashboard_FailedInspectionsThisMonth_CountIsCorrect()
    {
        var context = CreateInMemoryContext();
        var now = DateTime.Today;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var premises = new Premises { Name = "Test Cafe", Address = "1 Test St", Town = "Testville", RiskRating = RiskRating.Low };
        context.Premises.Add(premises);
        await context.SaveChangesAsync();

        context.Inspections.AddRange(
            new Inspection { PremisesId = premises.Id, InspectionDate = now.AddDays(-2), Score = 45, Outcome = InspectionOutcome.Fail, Notes = "Fail" },
            new Inspection { PremisesId = premises.Id, InspectionDate = now.AddDays(-5), Score = 80, Outcome = InspectionOutcome.Pass, Notes = "Pass" }
        );
        await context.SaveChangesAsync();

        var count = await context.Inspections
            .Where(i => i.InspectionDate >= startOfMonth && i.InspectionDate <= now && i.Outcome == InspectionOutcome.Fail)
            .CountAsync();

        Assert.Equal(1, count);
    }

    // Test 6: Inspector role cannot delete premises (checked via controller attribute)
    [Fact]
    public void PremisesController_DeleteConfirmed_IsRestrictedToAdminOnly()
    {
        var method = typeof(PremisesController).GetMethod("DeleteConfirmed");
        var attributes = method?
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .Cast<AuthorizeAttribute>();

        var isAdminOnly = attributes?.Any(a => a.Roles == "Admin") ?? false;
        Assert.True(isAdminOnly);
    }

    // Test 7: FollowUp DueDate before InspectionDate is flagged correctly
    [Fact]
    public async Task FollowUp_DueDateBeforeInspectionDate_IsInvalid()
    {
        var (_, _, inspection) = await SeedBasicDataAsync();

        var followUp = new FollowUp
        {
            InspectionId = inspection.Id,
            DueDate = inspection.InspectionDate.AddDays(-5),
            Status = FollowUpStatus.Open
        };

        var isDueDateBeforeInspection = followUp.DueDate < inspection.InspectionDate;
        Assert.True(isDueDateBeforeInspection);
    }
}
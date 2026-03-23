using FoodSafetyTracker.Domain.Entities;
using FoodSafetyTracker.Domain.Entities.Enums;
using Microsoft.AspNetCore.Identity;

namespace FoodSafetyTracker.MVC.Data;

public static class SeedData
{
    public static async Task InitialiseAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Seed Roles
        string[] roles = ["Admin", "Inspector", "Viewer"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Seed Admin User
        await SeedUserAsync(userManager, "admin@foodsafety.com", "Admin@123", "Admin");
        await SeedUserAsync(userManager, "inspector@foodsafety.com", "Inspector@123", "Inspector");
        await SeedUserAsync(userManager, "viewer@foodsafety.com", "Viewer@123", "Viewer");

        // Seed Premises, Inspections, FollowUps only if empty
        if (context.Premises.Any()) return;

        var premises = new List<Premises>
        {
            // Drogheda
            new() { Name = "The Harbour Cafe", Address = "1 Quay St", Town = "Drogheda", RiskRating = RiskRating.High },
            new() { Name = "River Bakery", Address = "22 West St", Town = "Drogheda", RiskRating = RiskRating.Medium },
            new() { Name = "Boyne Bistro", Address = "5 Bridge St", Town = "Drogheda", RiskRating = RiskRating.Low },
            new() { Name = "Market Deli", Address = "14 Shop St", Town = "Drogheda", RiskRating = RiskRating.Medium },
            // Dublin
            new() { Name = "Spire Sandwiches", Address = "3 O'Connell St", Town = "Dublin", RiskRating = RiskRating.Low },
            new() { Name = "Docklands Diner", Address = "8 Custom House Quay", Town = "Dublin", RiskRating = RiskRating.High },
            new() { Name = "Temple Eats", Address = "2 Temple Bar", Town = "Dublin", RiskRating = RiskRating.Medium },
            new() { Name = "Canal Kitchen", Address = "19 Grand Canal St", Town = "Dublin", RiskRating = RiskRating.Low },
            // Dundalk
            new() { Name = "Castle Cafe", Address = "7 Castle Rd", Town = "Dundalk", RiskRating = RiskRating.High },
            new() { Name = "Cooley Kitchen", Address = "3 Park St", Town = "Dundalk", RiskRating = RiskRating.Medium },
            new() { Name = "Louth Lunch Bar", Address = "11 Clanbrassil St", Town = "Dundalk", RiskRating = RiskRating.Low },
            new() { Name = "The Square Plate", Address = "6 Market Sq", Town = "Dundalk", RiskRating = RiskRating.High },
        };

        context.Premises.AddRange(premises);
        await context.SaveChangesAsync();

        var now = DateTime.Today;

        var inspections = new List<Inspection>
        {
            // This month
            new() { PremisesId = premises[0].Id, InspectionDate = now.AddDays(-2), Score = 45, Outcome = InspectionOutcome.Fail, Notes = "Poor hygiene standards found." },
            new() { PremisesId = premises[1].Id, InspectionDate = now.AddDays(-5), Score = 78, Outcome = InspectionOutcome.Pass, Notes = "Generally good." },
            new() { PremisesId = premises[2].Id, InspectionDate = now.AddDays(-7), Score = 91, Outcome = InspectionOutcome.Pass, Notes = "Excellent standards." },
            new() { PremisesId = premises[3].Id, InspectionDate = now.AddDays(-10), Score = 55, Outcome = InspectionOutcome.Fail, Notes = "Storage issues noted." },
            new() { PremisesId = premises[4].Id, InspectionDate = now.AddDays(-3), Score = 88, Outcome = InspectionOutcome.Pass, Notes = "Well maintained." },
            new() { PremisesId = premises[5].Id, InspectionDate = now.AddDays(-1), Score = 40, Outcome = InspectionOutcome.Fail, Notes = "Critical violations found." },
            new() { PremisesId = premises[6].Id, InspectionDate = now.AddDays(-8), Score = 72, Outcome = InspectionOutcome.Pass, Notes = "Minor issues only." },
            new() { PremisesId = premises[7].Id, InspectionDate = now.AddDays(-12), Score = 95, Outcome = InspectionOutcome.Pass, Notes = "Outstanding." },
            new() { PremisesId = premises[8].Id, InspectionDate = now.AddDays(-4), Score = 50, Outcome = InspectionOutcome.Fail, Notes = "Pest control required." },
            new() { PremisesId = premises[9].Id, InspectionDate = now.AddDays(-6), Score = 83, Outcome = InspectionOutcome.Pass, Notes = "Good overall." },
            new() { PremisesId = premises[10].Id, InspectionDate = now.AddMonths(-2), Score = 60, Outcome = InspectionOutcome.Pass, Notes = "Acceptable." },
            new() { PremisesId = premises[11].Id, InspectionDate = now.AddMonths(-2), Score = 35, Outcome = InspectionOutcome.Fail, Notes = "Multiple violations." },
            new() { PremisesId = premises[0].Id, InspectionDate = now.AddMonths(-3), Score = 70, Outcome = InspectionOutcome.Pass, Notes = "Improved since last visit." },
            new() { PremisesId = premises[1].Id, InspectionDate = now.AddMonths(-3), Score = 80, Outcome = InspectionOutcome.Pass, Notes = "Consistent standards." },
            new() { PremisesId = premises[2].Id, InspectionDate = now.AddMonths(-4), Score = 44, Outcome = InspectionOutcome.Fail, Notes = "Temperature violations." },
            new() { PremisesId = premises[3].Id, InspectionDate = now.AddMonths(-4), Score = 90, Outcome = InspectionOutcome.Pass, Notes = "Great improvement." },
            new() { PremisesId = premises[4].Id, InspectionDate = now.AddMonths(-5), Score = 55, Outcome = InspectionOutcome.Fail, Notes = "Cleaning schedule not followed." },
            new() { PremisesId = premises[5].Id, InspectionDate = now.AddMonths(-5), Score = 77, Outcome = InspectionOutcome.Pass, Notes = "Good." },
            new() { PremisesId = premises[6].Id, InspectionDate = now.AddMonths(-6), Score = 68, Outcome = InspectionOutcome.Pass, Notes = "Satisfactory." },
            new() { PremisesId = premises[7].Id, InspectionDate = now.AddMonths(-6), Score = 30, Outcome = InspectionOutcome.Fail, Notes = "Serious hygiene failures." },
            new() { PremisesId = premises[8].Id, InspectionDate = now.AddMonths(-7), Score = 85, Outcome = InspectionOutcome.Pass, Notes = "Well run kitchen." },
            new() { PremisesId = premises[9].Id, InspectionDate = now.AddMonths(-7), Score = 42, Outcome = InspectionOutcome.Fail, Notes = "Staff training needed." },
            new() { PremisesId = premises[10].Id, InspectionDate = now.AddMonths(-8), Score = 76, Outcome = InspectionOutcome.Pass, Notes = "Good practices observed." },
            new() { PremisesId = premises[11].Id, InspectionDate = now.AddMonths(-8), Score = 58, Outcome = InspectionOutcome.Fail, Notes = "Refrigeration issues." },
            new() { PremisesId = premises[0].Id, InspectionDate = now.AddMonths(-9), Score = 88, Outcome = InspectionOutcome.Pass, Notes = "No issues found." },
        };

        context.Inspections.AddRange(inspections);
        await context.SaveChangesAsync();

        var followUps = new List<FollowUp>
        {
            // Open and overdue
            new() { InspectionId = inspections[0].Id, DueDate = now.AddDays(-10), Status = FollowUpStatus.Open },
            new() { InspectionId = inspections[3].Id, DueDate = now.AddDays(-5), Status = FollowUpStatus.Open },
            new() { InspectionId = inspections[5].Id, DueDate = now.AddDays(-3), Status = FollowUpStatus.Open },
            new() { InspectionId = inspections[8].Id, DueDate = now.AddDays(-1), Status = FollowUpStatus.Open },
            // Open but not overdue
            new() { InspectionId = inspections[11].Id, DueDate = now.AddDays(5), Status = FollowUpStatus.Open },
            new() { InspectionId = inspections[14].Id, DueDate = now.AddDays(10), Status = FollowUpStatus.Open },
            // Closed
            new() { InspectionId = inspections[1].Id, DueDate = now.AddDays(-20), Status = FollowUpStatus.Closed, ClosedDate = now.AddDays(-15) },
            new() { InspectionId = inspections[2].Id, DueDate = now.AddDays(-30), Status = FollowUpStatus.Closed, ClosedDate = now.AddDays(-25) },
            new() { InspectionId = inspections[6].Id, DueDate = now.AddDays(-14), Status = FollowUpStatus.Closed, ClosedDate = now.AddDays(-10) },
            new() { InspectionId = inspections[9].Id, DueDate = now.AddDays(-7), Status = FollowUpStatus.Closed, ClosedDate = now.AddDays(-4) },
        };

        context.FollowUps.AddRange(followUps);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUserAsync(UserManager<IdentityUser> userManager, string email, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
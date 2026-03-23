namespace FoodSafetyTracker.MVC.ViewModels;

public class DashboardViewModel
{
    public int InspectionsThisMonth { get; set; }
    public int FailedInspectionsThisMonth { get; set; }
    public int OverdueFollowUps { get; set; }
    
    // Filter options
    public string? SelectedTown { get; set; }
    public string? SelectedRiskRating { get; set; }
    public IEnumerable<string> AvailableTowns { get; set; } = new List<string>();
}
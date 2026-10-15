using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // User, Admin
    public UserStage CurrentStage { get; set; } = UserStage.Idea;
    public UserObjective PrimaryObjective { get; set; } = UserObjective.BuildProduct;
    public PlatformType PreferredPlatform { get; set; } = PlatformType.X;
    public int HoursAvailablePerWeek { get; set; } = 10;
    public string SkillsAndConstraints { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public ICollection<ConnectedAccount> ConnectedAccounts { get; set; } = new List<ConnectedAccount>();
    public ICollection<UserWatchlist> Watchlist { get; set; } = new List<UserWatchlist>();
    public ICollection<UserGoal> Goals { get; set; } = new List<UserGoal>();
    public ICollection<ActionPlan> ActionPlans { get; set; } = new List<ActionPlan>();
}

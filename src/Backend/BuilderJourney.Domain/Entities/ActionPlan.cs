using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Domain.Entities;

public class UserWatchlist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public Guid BuilderProfileId { get; set; }
    public BuilderProfile BuilderProfile { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
}

public class UserGoal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public UserStage Stage { get; set; }
    public UserObjective Objective { get; set; }
    public int HoursPerWeek { get; set; }
    public string Skills { get; set; } = string.Empty;
    public string Constraints { get; set; } = string.Empty;
    public PlatformType TargetPlatform { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ActionPlan> ActionPlans { get; set; } = new List<ActionPlan>();
}

public class ActionPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserGoalId { get; set; }
    public UserGoal UserGoal { get; set; } = null!;

    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string MethodologyExplanation { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ActionPlanTask> Tasks { get; set; } = new List<ActionPlanTask>();
}

public class ActionPlanTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ActionPlanId { get; set; }
    public ActionPlan ActionPlan { get; set; } = null!;

    public int WeekNumber { get; set; }
    public string FocusArea { get; set; } = string.Empty; // e.g. "Validation & Pre-Launch"
    public string Title { get; set; } = string.Empty;
    public string ActionDescription { get; set; } = string.Empty;
    public string SuggestedContentFormat { get; set; } = string.Empty; // e.g. "1-Minute Loom Demo + Discussion Ask"
    public string MilestoneTarget { get; set; } = string.Empty;

    // Direct builder journey citation
    public Guid? RelevantBuilderProfileId { get; set; }
    public BuilderProfile? RelevantBuilderProfile { get; set; }
    public Guid? RelevantEventId { get; set; }
    public JourneyEvent? RelevantEvent { get; set; }
    public string SupportingRationale { get; set; } = string.Empty;
    public string SuccessMetric { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
}

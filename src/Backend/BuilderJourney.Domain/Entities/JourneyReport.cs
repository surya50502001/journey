using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Domain.Entities;

public class JourneyReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuilderProfileId { get; set; }
    public BuilderProfile BuilderProfile { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string ExecutiveSummary { get; set; } = string.Empty;
    public string ContentDnaSummary { get; set; } = string.Empty;
    public string KeyLessonsJson { get; set; } = "[]"; // Array of structured lessons with supporting evidence
    public string LimitationsDisclosed { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string Version { get; set; } = "1.0";

    public ReportSharingSetting? SharingSetting { get; set; }
}

public class ReportSharingSetting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JourneyReportId { get; set; }
    public JourneyReport JourneyReport { get; set; } = null!;

    public string ShareSlug { get; set; } = Guid.NewGuid().ToString("N")[..12];
    public VisibilityType Visibility { get; set; } = VisibilityType.Public;
    public bool AllowCloning { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class DataSyncJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuilderProfileId { get; set; }
    public BuilderProfile BuilderProfile { get; set; } = null!;

    public SyncStatus Status { get; set; } = SyncStatus.Queued;
    public int ItemsProcessed { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

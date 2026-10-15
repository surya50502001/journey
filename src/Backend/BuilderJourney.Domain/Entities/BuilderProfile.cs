using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Domain.Entities;

public class BuilderCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty; // SaaS, AI, Developer Tools, Open Source, Creator Business
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public ICollection<BuilderProfile> Profiles { get; set; } = new List<BuilderProfile>();
}

public class BuilderProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Handle { get; set; } = string.Empty; // e.g., @levelsio
    public string Name { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string ProfileUrl { get; set; } = string.Empty;
    public PlatformType Platform { get; set; } = PlatformType.X;
    
    public Guid BuilderCategoryId { get; set; }
    public BuilderCategory BuilderCategory { get; set; } = null!;

    public DateTime? DocumentedStartDate { get; set; }
    public string CurrentStatus { get; set; } = string.Empty; // e.g. "Bootstrapping PhotoAI & RemoteOK"
    public bool IsCurated { get; set; } = true;
    public bool IsVerified { get; set; } = false;
    
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int TotalPostsCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSyncedAt { get; set; }

    // Navigation
    public ICollection<ContentPost> Posts { get; set; } = new List<ContentPost>();
    public ICollection<AudienceMetricsSnapshot> AudienceSnapshots { get; set; } = new List<AudienceMetricsSnapshot>();
    public ICollection<JourneyEvent> JourneyEvents { get; set; } = new List<JourneyEvent>();
    public ICollection<GrowthInsight> GrowthInsights { get; set; } = new List<GrowthInsight>();
    public ICollection<UserWatchlist> WatchlistEntries { get; set; } = new List<UserWatchlist>();
    public ICollection<JourneyReport> Reports { get; set; } = new List<JourneyReport>();
    public ICollection<DataSyncJob> SyncJobs { get; set; } = new List<DataSyncJob>();
}

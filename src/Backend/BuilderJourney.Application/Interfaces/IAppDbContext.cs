using Microsoft.EntityFrameworkCore;
using BuilderJourney.Domain.Entities;

namespace BuilderJourney.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<ConnectedAccount> ConnectedAccounts { get; }
    DbSet<PlatformProvider> PlatformProviders { get; }
    DbSet<BuilderCategory> BuilderCategories { get; }
    DbSet<BuilderProfile> BuilderProfiles { get; }
    DbSet<ContentPost> ContentPosts { get; }
    DbSet<PostMetricsSnapshot> PostMetricsSnapshots { get; }
    DbSet<AudienceMetricsSnapshot> AudienceMetricsSnapshots { get; }
    DbSet<JourneyEvent> JourneyEvents { get; }
    DbSet<JourneyEventEvidence> JourneyEventEvidences { get; }
    DbSet<ContentClassification> ContentClassifications { get; }
    DbSet<GrowthInsight> GrowthInsights { get; }
    DbSet<InsightEvidence> InsightEvidences { get; }
    DbSet<UserWatchlist> UserWatchlists { get; }
    DbSet<UserGoal> UserGoals { get; }
    DbSet<ActionPlan> ActionPlans { get; }
    DbSet<ActionPlanTask> ActionPlanTasks { get; }
    DbSet<JourneyReport> JourneyReports { get; }
    DbSet<ReportSharingSetting> ReportSharingSettings { get; }
    DbSet<DataSyncJob> DataSyncJobs { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

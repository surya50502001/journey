using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Domain.Entities;

namespace BuilderJourney.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<ConnectedAccount> ConnectedAccounts => Set<ConnectedAccount>();
    public DbSet<PlatformProvider> PlatformProviders => Set<PlatformProvider>();
    public DbSet<BuilderCategory> BuilderCategories => Set<BuilderCategory>();
    public DbSet<BuilderProfile> BuilderProfiles => Set<BuilderProfile>();
    public DbSet<ContentPost> ContentPosts => Set<ContentPost>();
    public DbSet<PostMetricsSnapshot> PostMetricsSnapshots => Set<PostMetricsSnapshot>();
    public DbSet<AudienceMetricsSnapshot> AudienceMetricsSnapshots => Set<AudienceMetricsSnapshot>();
    public DbSet<JourneyEvent> JourneyEvents => Set<JourneyEvent>();
    public DbSet<JourneyEventEvidence> JourneyEventEvidences => Set<JourneyEventEvidence>();
    public DbSet<ContentClassification> ContentClassifications => Set<ContentClassification>();
    public DbSet<GrowthInsight> GrowthInsights => Set<GrowthInsight>();
    public DbSet<InsightEvidence> InsightEvidences => Set<InsightEvidence>();
    public DbSet<UserWatchlist> UserWatchlists => Set<UserWatchlist>();
    public DbSet<UserGoal> UserGoals => Set<UserGoal>();
    public DbSet<ActionPlan> ActionPlans => Set<ActionPlan>();
    public DbSet<ActionPlanTask> ActionPlanTasks => Set<ActionPlanTask>();
    public DbSet<JourneyReport> JourneyReports => Set<JourneyReport>();
    public DbSet<ReportSharingSetting> ReportSharingSettings => Set<ReportSharingSetting>();
    public DbSet<DataSyncJob> DataSyncJobs => Set<DataSyncJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.FullName).HasMaxLength(256);
        });

        // ConnectedAccount
        modelBuilder.Entity<ConnectedAccount>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => new { c.ApplicationUserId, c.Platform }).IsUnique();
            entity.HasOne(c => c.ApplicationUser)
                .WithMany(u => u.ConnectedAccounts)
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // BuilderCategory
        modelBuilder.Entity<BuilderCategory>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.Slug).IsUnique();
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Slug).IsRequired().HasMaxLength(100);
        });

        // BuilderProfile
        modelBuilder.Entity<BuilderProfile>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.HasIndex(b => new { b.Handle, b.Platform }).IsUnique();
            entity.Property(b => b.Handle).IsRequired().HasMaxLength(100);
            entity.Property(b => b.Name).IsRequired().HasMaxLength(200);

            entity.HasOne(b => b.BuilderCategory)
                .WithMany(c => c.Profiles)
                .HasForeignKey(b => b.BuilderCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ContentPost
        modelBuilder.Entity<ContentPost>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => new { p.BuilderProfileId, p.PlatformPostId }).IsUnique();
            entity.HasIndex(p => p.PublishedAt);

            entity.HasOne(p => p.BuilderProfile)
                .WithMany(b => b.Posts)
                .HasForeignKey(p => p.BuilderProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PostMetricsSnapshot
        modelBuilder.Entity<PostMetricsSnapshot>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => new { s.ContentPostId, s.CapturedAt });

            entity.HasOne(s => s.ContentPost)
                .WithMany(p => p.MetricSnapshots)
                .HasForeignKey(s => s.ContentPostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AudienceMetricsSnapshot
        modelBuilder.Entity<AudienceMetricsSnapshot>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => new { s.BuilderProfileId, s.CapturedAt });

            entity.HasOne(s => s.BuilderProfile)
                .WithMany(b => b.AudienceSnapshots)
                .HasForeignKey(s => s.BuilderProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // JourneyEvent
        modelBuilder.Entity<JourneyEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.BuilderProfileId, e.EventDate });

            entity.HasOne(e => e.BuilderProfile)
                .WithMany(b => b.JourneyEvents)
                .HasForeignKey(e => e.BuilderProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // JourneyEventEvidence
        modelBuilder.Entity<JourneyEventEvidence>(entity =>
        {
            entity.HasKey(ev => ev.Id);
            entity.HasOne(ev => ev.JourneyEvent)
                .WithMany(e => e.Evidences)
                .HasForeignKey(ev => ev.JourneyEventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ev => ev.ContentPost)
                .WithMany(p => p.EventEvidences)
                .HasForeignKey(ev => ev.ContentPostId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GrowthInsight
        modelBuilder.Entity<GrowthInsight>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.HasOne(g => g.BuilderProfile)
                .WithMany(b => b.GrowthInsights)
                .HasForeignKey(g => g.BuilderProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // InsightEvidence
        modelBuilder.Entity<InsightEvidence>(entity =>
        {
            entity.HasKey(ie => ie.Id);
            entity.HasOne(ie => ie.GrowthInsight)
                .WithMany(gi => gi.Evidences)
                .HasForeignKey(ie => ie.GrowthInsightId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ie => ie.ContentPost)
                .WithMany(p => p.InsightEvidences)
                .HasForeignKey(ie => ie.ContentPostId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // UserWatchlist
        modelBuilder.Entity<UserWatchlist>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.HasIndex(w => new { w.ApplicationUserId, w.BuilderProfileId }).IsUnique();

            entity.HasOne(w => w.ApplicationUser)
                .WithMany(u => u.Watchlist)
                .HasForeignKey(w => w.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(w => w.BuilderProfile)
                .WithMany(b => b.WatchlistEntries)
                .HasForeignKey(w => w.BuilderProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserGoal & ActionPlan
        modelBuilder.Entity<UserGoal>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.HasOne(g => g.ApplicationUser)
                .WithMany(u => u.Goals)
                .HasForeignKey(g => g.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActionPlan>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasOne(p => p.ApplicationUser)
                .WithMany(u => u.ActionPlans)
                .HasForeignKey(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.UserGoal)
                .WithMany(g => g.ActionPlans)
                .HasForeignKey(p => p.UserGoalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActionPlanTask>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasOne(t => t.ActionPlan)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ActionPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.RelevantBuilderProfile)
                .WithMany()
                .HasForeignKey(t => t.RelevantBuilderProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(t => t.RelevantEvent)
                .WithMany()
                .HasForeignKey(t => t.RelevantEventId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // JourneyReport & Sharing
        modelBuilder.Entity<JourneyReport>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasOne(r => r.BuilderProfile)
                .WithMany(b => b.Reports)
                .HasForeignKey(r => r.BuilderProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReportSharingSetting>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => s.ShareSlug).IsUnique();

            entity.HasOne(s => s.JourneyReport)
                .WithOne(r => r.SharingSetting)
                .HasForeignKey<ReportSharingSetting>(s => s.JourneyReportId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DataSyncJob
        modelBuilder.Entity<DataSyncJob>(entity =>
        {
            entity.HasKey(j => j.Id);
            entity.HasIndex(j => new { j.BuilderProfileId, j.StartedAt });

            entity.HasOne(j => j.BuilderProfile)
                .WithMany(b => b.SyncJobs)
                .HasForeignKey(j => j.BuilderProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

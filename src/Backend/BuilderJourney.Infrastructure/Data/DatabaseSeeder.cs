using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.Engines;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;
using BuilderJourney.Infrastructure.Providers;

namespace BuilderJourney.Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly AppDbContext _db;
    private readonly IJourneyReconstructionEngine _journeyEngine;
    private readonly IContentStrategyAnalyzer _contentAnalyzer;
    private readonly IBreakthroughDetector _breakthroughDetector;

    public DatabaseSeeder(
        AppDbContext db,
        IJourneyReconstructionEngine journeyEngine,
        IContentStrategyAnalyzer contentAnalyzer,
        IBreakthroughDetector breakthroughDetector)
    {
        _db = db;
        _journeyEngine = journeyEngine;
        _contentAnalyzer = contentAnalyzer;
        _breakthroughDetector = breakthroughDetector;
    }

    public async Task SeedAsync()
    {
        await _db.Database.EnsureCreatedAsync();

        // 1. Seed Categories
        if (!await _db.BuilderCategories.AnyAsync())
        {
            var categories = new List<BuilderCategory>
            {
                new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "SaaS", Slug = "saas", Description = "Software as a service products and subscription businesses.", DisplayOrder = 1 },
                new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "AI", Slug = "ai", Description = "Artificial intelligence apps, LLM wrappers, and generative tools.", DisplayOrder = 2 },
                new() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Developer Tools", Slug = "dev-tools", Description = "APIs, CLIs, libraries, and dev productivity utilities.", DisplayOrder = 3 },
                new() { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Open Source", Slug = "open-source", Description = "Open source projects building communities and commercial support.", DisplayOrder = 4 },
                new() { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Creator Business", Slug = "creator", Description = "Newsletters, digital products, content engines, and education.", DisplayOrder = 5 }
            };

            _db.BuilderCategories.AddRange(categories);
            await _db.SaveChangesAsync();
        }

        // 2. Seed Default User
        if (!await _db.Users.AnyAsync(u => u.Email == "demo@builderjourney.io"))
        {
            var demoUser = new ApplicationUser
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Email = "demo@builderjourney.io",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "Alex Carter",
                Role = "Admin",
                CurrentStage = UserStage.Prototype,
                PrimaryObjective = UserObjective.BuildProduct,
                PreferredPlatform = PlatformType.X,
                HoursAvailablePerWeek = 15,
                SkillsAndConstraints = "TypeScript, React, C#; building evenings and weekends.",
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(demoUser);
            await _db.SaveChangesAsync();
        }

        // 3. Seed Platform Providers
        if (!await _db.PlatformProviders.AnyAsync())
        {
            var providers = new List<PlatformProvider>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Platform = PlatformType.X,
                    Name = "X (Twitter)",
                    Description = "Official X API v2 integration for developer tweets, timestamps, and public metrics.",
                    ApiBaseUrl = "https://api.twitter.com/2",
                    IsEnabled = true,
                    SupportedMetricsSummary = "Likes, Replies, Reposts, Impressions, Bookmarks",
                    LimitationsNotice = "Historical follower counts are captured from initial tracking date forward."
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Platform = PlatformType.YouTube,
                    Name = "YouTube",
                    Description = "YouTube Data API v3 for video essays, devlogs, and subscriber milestones (Phase 4).",
                    ApiBaseUrl = "https://www.googleapis.com/youtube/v3",
                    IsEnabled = false,
                    SupportedMetricsSummary = "Views, Comments, Likes, Video Duration",
                    LimitationsNotice = "Scheduled for Phase 4 multi-platform rollout."
                }
            };

            _db.PlatformProviders.AddRange(providers);
            await _db.SaveChangesAsync();
        }

        // 4. Seed Curated Builders and Historical Posts
        var categoryMap = await _db.BuilderCategories.ToDictionaryAsync(c => c.Slug, c => c.Id);

        foreach (var curatedUser in CuratedPlatformDataProvider.CuratedBuilders)
        {
            var existingProfile = await _db.BuilderProfiles
                .Include(b => b.Posts)
                .Include(b => b.JourneyEvents)
                .FirstOrDefaultAsync(b => b.Handle.ToLower() == "@" + curatedUser.Username.ToLower());

            if (existingProfile == null)
            {
                var catSlug = curatedUser.Username switch
                {
                    "dannypostma" => "ai",
                    "tdinh_l" => "dev-tools",
                    "marc_louvion" => "saas",
                    _ => "saas"
                };

                var catId = categoryMap.TryGetValue(catSlug, out var cid) ? cid : categoryMap["saas"];

                var profile = new BuilderProfile
                {
                    Id = Guid.NewGuid(),
                    Handle = "@" + curatedUser.Username,
                    Name = curatedUser.DisplayName,
                    Bio = curatedUser.Bio,
                    AvatarUrl = curatedUser.AvatarUrl,
                    ProfileUrl = curatedUser.ProfileUrl,
                    Platform = PlatformType.X,
                    BuilderCategoryId = catId,
                    DocumentedStartDate = curatedUser.AccountCreatedAt,
                    CurrentStatus = curatedUser.Username switch
                    {
                        "levelsio" => "Bootstrapping Photo AI ($1M+ ARR) and Nomad List",
                        "marc_louvion" => "Growing ShipFast ($50k+ MRR) & solo startup studio",
                        "dannypostma" => "Scaling HeadshotPro to $3M+ ARR",
                        "tdinh_l" => "Operating TypingMind ($100k+ ARR) & DevUtils",
                        _ => "Building products in public"
                    },
                    IsCurated = true,
                    IsVerified = true,
                    FollowersCount = curatedUser.FollowersCount,
                    FollowingCount = curatedUser.FollowingCount,
                    TotalPostsCount = curatedUser.TotalPostsCount,
                    CreatedAt = DateTime.UtcNow,
                    LastSyncedAt = DateTime.UtcNow
                };

                _db.BuilderProfiles.Add(profile);

                // Add posts
                if (CuratedPlatformDataProvider.CuratedPosts.TryGetValue(curatedUser.ProviderUserId, out var postList))
                {
                    foreach (var p in postList)
                    {
                        var postEntity = new ContentPost
                        {
                            Id = Guid.NewGuid(),
                            BuilderProfileId = profile.Id,
                            BuilderProfile = profile,
                            PlatformPostId = p.PlatformPostId,
                            Content = p.Content,
                            PublishedAt = p.PublishedAt,
                            PostUrl = p.PostUrl,
                            PostType = p.PostType,
                            MediaUrlsJson = JsonSerializer.Serialize(p.MediaUrls),
                            Likes = p.Likes,
                            Replies = p.Replies,
                            Reposts = p.Reposts,
                            Views = p.Views,
                            Bookmarks = p.Bookmarks
                        };

                        postEntity.ClassifiedCategory = _contentAnalyzer.ClassifyPost(postEntity);

                        // Metric snapshot
                        postEntity.MetricSnapshots.Add(new PostMetricsSnapshot
                        {
                            Id = Guid.NewGuid(),
                            ContentPostId = postEntity.Id,
                            ContentPost = postEntity,
                            Likes = p.Likes,
                            Replies = p.Replies,
                            Reposts = p.Reposts,
                            Views = p.Views,
                            Bookmarks = p.Bookmarks,
                            CapturedAt = p.PublishedAt.AddHours(24)
                        });

                        profile.Posts.Add(postEntity);
                    }
                }

                // Audience snapshot
                profile.AudienceSnapshots.Add(new AudienceMetricsSnapshot
                {
                    Id = Guid.NewGuid(),
                    BuilderProfileId = profile.Id,
                    BuilderProfile = profile,
                    FollowerCount = profile.FollowersCount,
                    FollowingCount = profile.FollowingCount,
                    ListedCount = 1200,
                    TotalPostsCount = profile.TotalPostsCount,
                    CapturedAt = DateTime.UtcNow
                });

                // Reconstruct journey events
                var reconstructedEvents = _journeyEngine.ReconstructJourney(profile, profile.Posts);
                foreach (var ev in reconstructedEvents)
                {
                    profile.JourneyEvents.Add(ev);
                }

                if (reconstructedEvents.Count > 0)
                {
                    profile.DocumentedStartDate = reconstructedEvents.Min(e => e.EventDate);
                }

                await _db.SaveChangesAsync();
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BuilderJourney.Application.Engines;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;
using BuilderJourney.Domain.Interfaces;

namespace BuilderJourney.Infrastructure.Background;

public class BackgroundSyncWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundSyncWorker> _logger;

    public BackgroundSyncWorker(
        IServiceProvider serviceProvider,
        ILogger<BackgroundSyncWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Builder Journey Background Data Sync Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
                var providers = scope.ServiceProvider.GetRequiredService<IEnumerable<IPlatformDataProvider>>();
                var engine = scope.ServiceProvider.GetRequiredService<IJourneyReconstructionEngine>();
                var contentAnalyzer = scope.ServiceProvider.GetRequiredService<IContentStrategyAnalyzer>();

                var pendingJob = await db.DataSyncJobs
                    .Include(j => j.BuilderProfile)
                        .ThenInclude(b => b.Posts)
                    .Where(j => j.Status == SyncStatus.Queued)
                    .OrderBy(j => j.StartedAt)
                    .FirstOrDefaultAsync(stoppingToken);

                if (pendingJob != null)
                {
                    _logger.LogInformation("Processing sync job {JobId} for builder {Handle}", pendingJob.Id, pendingJob.BuilderProfile.Handle);
                    pendingJob.Status = SyncStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);

                    var builder = pendingJob.BuilderProfile;
                    var provider = providers.FirstOrDefault(p => p.Platform == builder.Platform)
                                   ?? providers.FirstOrDefault(p => p.Platform == PlatformType.Curated);

                    if (provider != null)
                    {
                        var postsData = await provider.GetUserPostsAsync(builder.Handle.TrimStart('@'), null, 50, stoppingToken);
                        var newItemsCount = 0;

                        foreach (var p in postsData)
                        {
                            var existing = builder.Posts.FirstOrDefault(ep => ep.PlatformPostId == p.PlatformPostId);
                            if (existing == null)
                            {
                                var newPost = new ContentPost
                                {
                                    Id = Guid.NewGuid(),
                                    BuilderProfileId = builder.Id,
                                    BuilderProfile = builder,
                                    PlatformPostId = p.PlatformPostId,
                                    Content = p.Content,
                                    PublishedAt = p.PublishedAt,
                                    PostUrl = p.PostUrl,
                                    PostType = p.PostType,
                                    MediaUrlsJson = System.Text.Json.JsonSerializer.Serialize(p.MediaUrls),
                                    Likes = p.Likes,
                                    Replies = p.Replies,
                                    Reposts = p.Reposts,
                                    Views = p.Views,
                                    Bookmarks = p.Bookmarks
                                };

                                newPost.ClassifiedCategory = contentAnalyzer.ClassifyPost(newPost);
                                builder.Posts.Add(newPost);
                                newItemsCount++;
                            }
                            else
                            {
                                existing.Likes = p.Likes;
                                existing.Replies = p.Replies;
                                existing.Reposts = p.Reposts;
                                existing.Views = p.Views;
                                existing.Bookmarks = p.Bookmarks;
                            }
                        }

                        // Reconstruct journey with fresh posts
                        var events = engine.ReconstructJourney(builder, builder.Posts);
                        var existingEvents = await db.JourneyEvents.Where(e => e.BuilderProfileId == builder.Id).ToListAsync(stoppingToken);
                        if (existingEvents.Count > 0)
                        {
                            db.JourneyEvents.RemoveRange(existingEvents);
                        }

                        foreach (var ev in events)
                        {
                            db.JourneyEvents.Add(ev);
                        }

                        builder.LastSyncedAt = DateTime.UtcNow;
                        pendingJob.Status = SyncStatus.Completed;
                        pendingJob.ItemsProcessed = newItemsCount;
                        pendingJob.CompletedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        pendingJob.Status = SyncStatus.Failed;
                        pendingJob.ErrorMessage = $"No platform provider registered for {builder.Platform}";
                        pendingJob.CompletedAt = DateTime.UtcNow;
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during background data synchronization job.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}

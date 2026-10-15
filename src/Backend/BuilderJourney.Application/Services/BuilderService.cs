using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.Services;

public class BuilderService : IBuilderService
{
    private readonly IAppDbContext _db;

    public BuilderService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<PagedResult<BuilderProfileSummaryDto>>> GetBuildersAsync(BuilderFilterDto filter, CancellationToken ct = default)
    {
        var query = _db.BuilderProfiles
            .Include(b => b.BuilderCategory)
            .Include(b => b.JourneyEvents)
            .Include(b => b.Posts)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
        {
            var search = filter.SearchQuery.Trim().ToLowerInvariant();
            query = query.Where(b =>
                b.Name.ToLower().Contains(search) ||
                b.Handle.ToLower().Contains(search) ||
                b.Bio.ToLower().Contains(search) ||
                b.CurrentStatus.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filter.CategorySlug) && filter.CategorySlug != "all")
        {
            query = query.Where(b => b.BuilderCategory.Slug.ToLower() == filter.CategorySlug.ToLower());
        }

        if (filter.Platform.HasValue)
        {
            query = query.Where(b => b.Platform == filter.Platform.Value);
        }

        if (filter.CuratedOnly.HasValue && filter.CuratedOnly.Value)
        {
            query = query.Where(b => b.IsCurated);
        }

        query = filter.SortBy?.ToLower() switch
        {
            "followers" => query.OrderByDescending(b => b.FollowersCount),
            "milestones" => query.OrderByDescending(b => b.JourneyEvents.Count),
            "name" => query.OrderBy(b => b.Name),
            "recent" => query.OrderByDescending(b => b.CreatedAt),
            _ => query.OrderByDescending(b => b.IsCurated).ThenByDescending(b => b.FollowersCount)
        };

        var totalCount = await query.CountAsync(ct);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BuilderProfileSummaryDto(
                b.Id,
                b.Handle,
                b.Name,
                b.Bio,
                b.AvatarUrl,
                b.ProfileUrl,
                b.Platform,
                b.BuilderCategory.Name,
                b.BuilderCategory.Slug,
                b.CurrentStatus,
                b.IsCurated,
                b.IsVerified,
                b.FollowersCount,
                b.Posts.Count,
                b.JourneyEvents.Count,
                b.DocumentedStartDate,
                b.LastSyncedAt
            ))
            .ToListAsync(ct);

        var result = new PagedResult<BuilderProfileSummaryDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponse<PagedResult<BuilderProfileSummaryDto>>.Ok(result);
    }

    public async Task<ApiResponse<BuilderProfileDetailDto>> GetBuilderByIdAsync(Guid id, Guid? currentUserId = null, CancellationToken ct = default)
    {
        var builder = await _db.BuilderProfiles
            .Include(b => b.BuilderCategory)
            .Include(b => b.JourneyEvents)
            .Include(b => b.GrowthInsights)
            .Include(b => b.Posts)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (builder == null)
        {
            return ApiResponse<BuilderProfileDetailDto>.Fail("Builder profile not found.");
        }

        var isWatched = false;
        if (currentUserId.HasValue)
        {
            isWatched = await _db.UserWatchlists
                .AnyAsync(w => w.ApplicationUserId == currentUserId.Value && w.BuilderProfileId == id, ct);
        }

        var dto = new BuilderProfileDetailDto(
            builder.Id,
            builder.Handle,
            builder.Name,
            builder.Bio,
            builder.AvatarUrl,
            builder.ProfileUrl,
            builder.Platform,
            builder.BuilderCategoryId,
            builder.BuilderCategory?.Name ?? "General",
            builder.BuilderCategory?.Slug ?? "general",
            builder.CurrentStatus,
            builder.IsCurated,
            builder.IsVerified,
            builder.FollowersCount,
            builder.FollowingCount,
            builder.Posts.Count,
            builder.DocumentedStartDate,
            builder.CreatedAt,
            builder.LastSyncedAt,
            builder.JourneyEvents.Count,
            builder.GrowthInsights.Count,
            isWatched
        );

        return ApiResponse<BuilderProfileDetailDto>.Ok(dto);
    }

    public async Task<ApiResponse<BuilderProfileDetailDto>> GetBuilderByHandleAsync(string handle, Guid? currentUserId = null, CancellationToken ct = default)
    {
        var cleanHandle = handle.Trim().TrimStart('@').ToLowerInvariant();
        var builder = await _db.BuilderProfiles
            .Include(b => b.BuilderCategory)
            .Include(b => b.JourneyEvents)
            .Include(b => b.GrowthInsights)
            .Include(b => b.Posts)
            .FirstOrDefaultAsync(b => b.Handle.ToLower() == "@" + cleanHandle || b.Handle.ToLower() == cleanHandle, ct);

        if (builder == null)
        {
            return ApiResponse<BuilderProfileDetailDto>.Fail($"Builder profile for @{cleanHandle} not found.");
        }

        return await GetBuilderByIdAsync(builder.Id, currentUserId, ct);
    }

    public async Task<ApiResponse<List<BuilderCategoryDto>>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var categories = await _db.BuilderCategories
            .Include(c => c.Profiles)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new BuilderCategoryDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.DisplayOrder,
                c.Profiles.Count
            ))
            .ToListAsync(ct);

        return ApiResponse<List<BuilderCategoryDto>>.Ok(categories);
    }

    public async Task<ApiResponse<BuilderProfileDetailDto>> CreateCustomBuilderAsync(CreateCustomBuilderRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Handle) || string.IsNullOrWhiteSpace(request.Name))
        {
            return ApiResponse<BuilderProfileDetailDto>.Fail("Handle and Name are required.");
        }

        var cleanHandle = request.Handle.Trim();
        if (!cleanHandle.StartsWith("@")) cleanHandle = "@" + cleanHandle;

        var existing = await _db.BuilderProfiles.FirstOrDefaultAsync(b => b.Handle.ToLower() == cleanHandle.ToLower(), ct);
        if (existing != null)
        {
            return await GetBuilderByIdAsync(existing.Id, null, ct);
        }

        var category = await _db.BuilderCategories.FindAsync(new object[] { request.CategoryId }, ct);
        if (category == null)
        {
            category = await _db.BuilderCategories.FirstOrDefaultAsync(ct)
                       ?? new BuilderCategory { Name = "SaaS", Slug = "saas" };
        }

        var profile = new BuilderProfile
        {
            Id = Guid.NewGuid(),
            Handle = cleanHandle,
            Name = request.Name.Trim(),
            Bio = request.Bio?.Trim() ?? string.Empty,
            ProfileUrl = string.IsNullOrWhiteSpace(request.ProfileUrl) ? $"https://x.com/{cleanHandle.TrimStart('@')}" : request.ProfileUrl.Trim(),
            AvatarUrl = $"https://unavatar.io/twitter/{cleanHandle.TrimStart('@')}",
            Platform = request.Platform,
            BuilderCategoryId = category.Id,
            BuilderCategory = category,
            CurrentStatus = request.CurrentStatus?.Trim() ?? "Building in public",
            IsCurated = false,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.BuilderProfiles.Add(profile);

        // Queue initial sync job
        var syncJob = new DataSyncJob
        {
            Id = Guid.NewGuid(),
            BuilderProfileId = profile.Id,
            BuilderProfile = profile,
            Status = SyncStatus.Queued,
            StartedAt = DateTime.UtcNow
        };
        _db.DataSyncJobs.Add(syncJob);

        await _db.SaveChangesAsync(ct);

        return await GetBuilderByIdAsync(profile.Id, null, ct);
    }

    public async Task<ApiResponse<SyncJobStatusDto>> TriggerSyncAsync(Guid builderId, SyncBuilderRequest request, CancellationToken ct = default)
    {
        var builder = await _db.BuilderProfiles.FindAsync(new object[] { builderId }, ct);
        if (builder == null)
        {
            return ApiResponse<SyncJobStatusDto>.Fail("Builder not found.");
        }

        var job = new DataSyncJob
        {
            Id = Guid.NewGuid(),
            BuilderProfileId = builder.Id,
            BuilderProfile = builder,
            Status = SyncStatus.Queued,
            StartedAt = DateTime.UtcNow
        };

        _db.DataSyncJobs.Add(job);
        await _db.SaveChangesAsync(ct);

        var dto = new SyncJobStatusDto(
            job.Id,
            builder.Id,
            builder.Handle,
            job.Status,
            0,
            null,
            job.StartedAt,
            null
        );

        return ApiResponse<SyncJobStatusDto>.Ok(dto, "Data synchronization job queued.");
    }
}

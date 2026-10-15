using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.DTOs;

public record BuilderCategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    int DisplayOrder,
    int BuilderCount
);

public record BuilderProfileSummaryDto(
    Guid Id,
    string Handle,
    string Name,
    string Bio,
    string AvatarUrl,
    string ProfileUrl,
    PlatformType Platform,
    string CategoryName,
    string CategorySlug,
    string CurrentStatus,
    bool IsCurated,
    bool IsVerified,
    int FollowersCount,
    int TotalPostsCount,
    int DocumentedMilestonesCount,
    DateTime? DocumentedStartDate,
    DateTime? LastSyncedAt
);

public record BuilderProfileDetailDto(
    Guid Id,
    string Handle,
    string Name,
    string Bio,
    string AvatarUrl,
    string ProfileUrl,
    PlatformType Platform,
    Guid CategoryId,
    string CategoryName,
    string CategorySlug,
    string CurrentStatus,
    bool IsCurated,
    bool IsVerified,
    int FollowersCount,
    int FollowingCount,
    int TotalPostsCount,
    DateTime? DocumentedStartDate,
    DateTime CreatedAt,
    DateTime? LastSyncedAt,
    int TotalMilestones,
    int TotalInsights,
    bool IsInUserWatchlist
);

public record BuilderFilterDto(
    string? SearchQuery,
    string? CategorySlug,
    PlatformType? Platform,
    bool? CuratedOnly,
    string? SortBy, // "followers", "milestones", "name", "recent"
    int Page = 1,
    int PageSize = 12
);

public record CreateCustomBuilderRequest(
    string Handle,
    string Name,
    string Bio,
    string ProfileUrl,
    PlatformType Platform,
    Guid CategoryId,
    string CurrentStatus
);

public record SyncBuilderRequest(
    bool ForceFullResync = false
);

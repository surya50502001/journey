using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Domain.Interfaces;

public record PlatformUserProfile(
    string ProviderUserId,
    string Username,
    string DisplayName,
    string Bio,
    string AvatarUrl,
    string ProfileUrl,
    int FollowersCount,
    int FollowingCount,
    int TotalPostsCount,
    DateTime? AccountCreatedAt
);

public record PlatformPostData(
    string PlatformPostId,
    string Content,
    DateTime PublishedAt,
    string PostUrl,
    string PostType,
    List<string> MediaUrls,
    int Likes,
    int Replies,
    int Reposts,
    int Views,
    int Bookmarks
);

public record PlatformAudienceMetrics(
    int FollowerCount,
    int FollowingCount,
    int ListedCount,
    int TotalPostsCount,
    DateTime CapturedAt
);

public interface IPlatformDataProvider
{
    PlatformType Platform { get; }
    PlatformCapability GetCapabilities();
    Task<PlatformUserProfile?> GetUserProfileAsync(string handleOrId, CancellationToken ct = default);
    Task<IReadOnlyList<PlatformPostData>> GetUserPostsAsync(string providerUserId, DateTime? since = null, int maxResults = 100, CancellationToken ct = default);
    Task<PlatformAudienceMetrics?> GetAudienceMetricsAsync(string providerUserId, CancellationToken ct = default);
}

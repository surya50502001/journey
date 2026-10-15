using System.Net.Http.Headers;
using System.Text.Json;
using BuilderJourney.Domain.Enums;
using BuilderJourney.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BuilderJourney.Infrastructure.Providers;

public class XPlatformDataProvider : IPlatformDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<XPlatformDataProvider> _logger;

    public PlatformType Platform => PlatformType.X;

    public XPlatformDataProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<XPlatformDataProvider> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://api.twitter.com/");
        var bearerToken = _configuration["X:BearerToken"];
        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
    }

    public PlatformCapability GetCapabilities()
    {
        return new PlatformCapability
        {
            Platform = PlatformType.X,
            SupportsLikes = true,
            SupportsReplies = true,
            SupportsReposts = true,
            SupportsViews = true,
            SupportsBookmarks = true,
            SupportsHistoricalFollowers = false, // X API v2 only returns current public metrics
            SupportsFullHistory = false, // Free/Basic v2 access is limited to recent 3200 or 7-day search
            RateLimitDescription = "15 requests per 15-minute window for User Lookup (App rate limit on v2)",
            DataRetentionPolicy = "Content displayed must reflect author deletions within 24 hours per X Developer Agreement.",
            AttributionRequirement = "Direct link to original post on x.com and author username display.",
            Limitations = new List<string>
            {
                "Historical follower count snapshots cannot be reconstructed prior to the first time the account was tracked.",
                "Private messages, bookmark owners, and tweet impressions from accounts with protected metrics are strictly inaccessible.",
                "Rate limit status must be tracked via 'x-rate-limit-remaining' response headers."
            }
        };
    }

    public async Task<PlatformUserProfile?> GetUserProfileAsync(string handleOrId, CancellationToken ct = default)
    {
        var cleanHandle = handleOrId.Trim().TrimStart('@');
        var bearer = _configuration["X:BearerToken"];
        if (string.IsNullOrWhiteSpace(bearer))
        {
            _logger.LogWarning("X API Bearer token is not configured. Live lookup cannot proceed.");
            return null;
        }

        try
        {
            var url = $"2/users/by/username/{cleanHandle}?user.fields=created_at,description,profile_image_url,public_metrics,verified";
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("X API user lookup failed for {Handle} with status code {Code}", cleanHandle, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("data", out var data)) return null;

            var id = data.GetProperty("id").GetString() ?? "";
            var name = data.GetProperty("name").GetString() ?? cleanHandle;
            var username = data.GetProperty("username").GetString() ?? cleanHandle;
            var description = data.TryGetProperty("description", out var descProp) ? descProp.GetString() ?? "" : "";
            var avatarUrl = data.TryGetProperty("profile_image_url", out var imgProp) ? imgProp.GetString() ?? "" : "";
            
            var followers = 0;
            var following = 0;
            var tweetCount = 0;
            if (data.TryGetProperty("public_metrics", out var metrics))
            {
                followers = metrics.GetProperty("followers_count").GetInt32();
                following = metrics.GetProperty("following_count").GetInt32();
                tweetCount = metrics.GetProperty("tweet_count").GetInt32();
            }

            DateTime? createdAt = null;
            if (data.TryGetProperty("created_at", out var createdProp) && DateTime.TryParse(createdProp.GetString(), out var parsedDate))
            {
                createdAt = parsedDate;
            }

            return new PlatformUserProfile(
                id,
                username,
                name,
                description,
                avatarUrl,
                $"https://x.com/{username}",
                followers,
                following,
                tweetCount,
                createdAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying X API for user {Handle}", cleanHandle);
            return null;
        }
    }

    public async Task<IReadOnlyList<PlatformPostData>> GetUserPostsAsync(string providerUserId, DateTime? since = null, int maxResults = 100, CancellationToken ct = default)
    {
        var bearer = _configuration["X:BearerToken"];
        if (string.IsNullOrWhiteSpace(bearer))
        {
            return Array.Empty<PlatformPostData>();
        }

        try
        {
            var clampedResults = Math.Clamp(maxResults, 5, 100);
            var url = $"2/users/{providerUserId}/tweets?max_results={clampedResults}&tweet.fields=created_at,public_metrics,entities,attachments&expansions=attachments.media_keys&media.fields=url,preview_image_url,type";
            if (since.HasValue)
            {
                url += $"&start_time={since.Value:yyyy-MM-ddTHH:mm:ssZ}";
            }

            var response = await _httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("X API tweet timeline failed with status {Code}", response.StatusCode);
                return Array.Empty<PlatformPostData>();
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            {
                return Array.Empty<PlatformPostData>();
            }

            var posts = new List<PlatformPostData>();
            foreach (var item in data.EnumerateArray())
            {
                var id = item.GetProperty("id").GetString() ?? "";
                var text = item.GetProperty("text").GetString() ?? "";
                var createdAt = item.TryGetProperty("created_at", out var cProp) && DateTime.TryParse(cProp.GetString(), out var dt) ? dt : DateTime.UtcNow;

                var likes = 0;
                var replies = 0;
                var reposts = 0;
                var views = 0;
                var bookmarks = 0;

                if (item.TryGetProperty("public_metrics", out var m))
                {
                    likes = m.TryGetProperty("like_count", out var lProp) ? lProp.GetInt32() : 0;
                    replies = m.TryGetProperty("reply_count", out var rProp) ? rProp.GetInt32() : 0;
                    reposts = m.TryGetProperty("retweet_count", out var rtProp) ? rtProp.GetInt32() : 0;
                    views = m.TryGetProperty("impression_count", out var vProp) ? vProp.GetInt32() : 0;
                    bookmarks = m.TryGetProperty("bookmark_count", out var bProp) ? bProp.GetInt32() : 0;
                }

                posts.Add(new PlatformPostData(
                    id,
                    text,
                    createdAt,
                    $"https://x.com/i/status/{id}",
                    "Tweet",
                    new List<string>(),
                    likes,
                    replies,
                    reposts,
                    views,
                    bookmarks
                ));
            }

            return posts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweets for user ID {UserId}", providerUserId);
            return Array.Empty<PlatformPostData>();
        }
    }

    public Task<PlatformAudienceMetrics?> GetAudienceMetricsAsync(string providerUserId, CancellationToken ct = default)
    {
        // Follower history is not provided by Twitter API v2; current snapshot is provided via GetUserProfileAsync
        return Task.FromResult<PlatformAudienceMetrics?>(null);
    }
}

using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.DTOs;

public record CategoryDistributionDto(
    ContentCategory Category,
    string CategoryName,
    int PostCount,
    double Percentage,
    double AverageLikes,
    double AverageReplies,
    double AverageReposts
);

public record PostingCadenceDto(
    string PeriodLabel, // e.g., "2023-Q1" or "2023-01"
    int PostCount,
    double AverageLikes,
    double AverageReplies
);

public record FormatPerformanceDto(
    string FormatName, // "Product Demos", "Educational Threads", "Build-in-Public Text", "Media/Screenshots"
    int Count,
    double AverageLikes,
    double AverageReplies,
    double AverageReposts,
    double EngagementMultiplier // Relative to typical post
);

public record TopTopicDto(
    string Topic,
    int Frequency,
    double AverageEngagement,
    List<string> SamplePostSnippets
);

public record ContentStrategySummaryDto(
    Guid BuilderId,
    string BuilderName,
    string BuilderHandle,
    int TotalAnalyzedPosts,
    DateTime? EarliestAnalyzedPost,
    DateTime? LatestAnalyzedPost,
    double AveragePostsPerWeek,
    string ConsistencyScore, // "High (Consistent cadence)", "Moderate", "Variable"
    List<CategoryDistributionDto> Categories,
    List<PostingCadenceDto> CadenceHistory,
    List<FormatPerformanceDto> FormatPerformances,
    List<TopTopicDto> TopTopics,
    List<PostSummaryDto> TopResonancePosts,
    string StrategyOverview,
    string LimitationsNotice
);

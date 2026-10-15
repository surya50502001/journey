using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.DTOs;

public record BreakthroughEvidenceDto(
    Guid PostId,
    string Content,
    DateTime PublishedAt,
    string PostUrl,
    int Likes,
    int Replies,
    string RoleInShift // "Before shift baseline", "Key inflection post", "Sustained pattern post"
);

public record BreakthroughDto(
    Guid Id,
    string Title,
    string Summary,
    string MetricObserved,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    string MagnitudeChange,
    string PlausibleExplanations,
    string LimitationsAndAlternatives,
    List<BreakthroughEvidenceDto> Evidences
);

public record BuilderComparisonRequest(
    List<Guid> BuilderIds
);

public record BuilderComparisonMetric(
    string MetricName,
    Dictionary<Guid, string> ValuesByBuilderId,
    string ContextNote
);

public record BuilderComparisonProfileDto(
    Guid Id,
    string Name,
    string Handle,
    string AvatarUrl,
    string CategoryName,
    int FollowersCount,
    int TotalPostsCount,
    int DocumentedMilestonesCount,
    DateTime? DocumentedStartDate,
    string PrimaryContentStyle,
    string LaunchPacing
);

public record BuilderComparisonResultDto(
    List<BuilderComparisonProfileDto> Builders,
    List<BuilderComparisonMetric> ComparativeMetrics,
    Dictionary<Guid, List<CategoryDistributionDto>> CategoryMixByBuilder,
    Dictionary<Guid, List<JourneyEventDto>> AlignedMilestonesByBuilder,
    string ComparativeSynthesis,
    string AnalyticalLimitationsNotice
);

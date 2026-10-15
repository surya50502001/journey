using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.DTOs;

public record GenerateActionPlanRequest(
    UserStage Stage,
    UserObjective Objective,
    int HoursPerWeek,
    string Skills,
    string Constraints,
    PlatformType TargetPlatform
);

public record ActionPlanTaskDto(
    Guid Id,
    int WeekNumber,
    string FocusArea,
    string Title,
    string ActionDescription,
    string SuggestedContentFormat,
    string MilestoneTarget,
    Guid? RelevantBuilderProfileId,
    string? RelevantBuilderName,
    string? RelevantBuilderHandle,
    Guid? RelevantEventId,
    string? RelevantEventTitle,
    string SupportingRationale,
    string SuccessMetric,
    bool IsCompleted
);

public record ActionPlanDto(
    Guid Id,
    Guid UserGoalId,
    UserStage Stage,
    UserObjective Objective,
    int HoursPerWeek,
    PlatformType TargetPlatform,
    string Title,
    string Summary,
    string MethodologyExplanation,
    DateTime GeneratedAt,
    List<ActionPlanTaskDto> Tasks
);

public record UpdateTaskStatusRequest(
    bool IsCompleted
);

public record WatchlistEntryDto(
    Guid Id,
    Guid BuilderProfileId,
    string BuilderName,
    string BuilderHandle,
    string AvatarUrl,
    string CategoryName,
    int FollowersCount,
    string CurrentStatus,
    DateTime AddedAt,
    string Notes,
    DateTime? LatestMilestoneDate,
    string? LatestMilestoneTitle
);

public record AddToWatchlistRequest(
    Guid BuilderProfileId,
    string? Notes
);

public record LessonLearnedDto(
    string Principle,
    string BuilderContext,
    string PracticalApplication,
    string SupportingEvidence
);

public record JourneyReportDto(
    Guid Id,
    Guid BuilderProfileId,
    string BuilderName,
    string BuilderHandle,
    string AvatarUrl,
    string Bio,
    string CategoryName,
    string CurrentStatus,
    DateTime? DocumentedStartDate,
    string Title,
    string ExecutiveSummary,
    string ContentDnaSummary,
    List<LessonLearnedDto> KeyLessons,
    string LimitationsDisclosed,
    DateTime GeneratedAt,
    string Version,
    string? ShareSlug,
    VisibilityType Visibility,
    JourneyTimelineDto Timeline,
    ContentStrategySummaryDto ContentStrategy,
    List<BreakthroughDto> Breakthroughs
);

public record UpdateSharingSettingsRequest(
    VisibilityType Visibility,
    bool AllowCloning
);

public record SyncJobStatusDto(
    Guid JobId,
    Guid BuilderProfileId,
    string BuilderHandle,
    SyncStatus Status,
    int ItemsProcessed,
    string? ErrorMessage,
    DateTime StartedAt,
    DateTime? CompletedAt
);

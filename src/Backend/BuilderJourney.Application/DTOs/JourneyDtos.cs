using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.DTOs;

public record JourneyEventEvidenceDto(
    Guid Id,
    Guid ContentPostId,
    string PostContent,
    DateTime PublishedAt,
    string PostUrl,
    string PostType,
    int Likes,
    int Replies,
    int Reposts,
    EvidenceType EvidenceType,
    string Excerpt,
    double Weight
);

public record JourneyEventDto(
    Guid Id,
    string Title,
    string Description,
    DateTime EventDate,
    DatePrecision Precision,
    JourneyEventCategory Category,
    string CategoryName,
    ConfidenceLevel Confidence,
    bool IsInferred,
    string LimitationsNote,
    List<JourneyEventEvidenceDto> Evidences
);

public record JourneyTimelineDto(
    Guid BuilderId,
    string BuilderName,
    string BuilderHandle,
    string AvatarUrl,
    DateTime? DocumentedStartDate,
    string CurrentStatus,
    int TotalEvents,
    int HighConfidenceEventsCount,
    int InferredEventsCount,
    List<JourneyEventDto> Events,
    List<string> ReconstructionLimitations
);

public record PostSummaryDto(
    Guid Id,
    string PlatformPostId,
    string Content,
    DateTime PublishedAt,
    string PostUrl,
    string PostType,
    ContentCategory? ClassifiedCategory,
    int Likes,
    int Replies,
    int Reposts,
    int Views,
    int Bookmarks
);

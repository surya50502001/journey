using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Domain.Entities;

public class ContentPost
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuilderProfileId { get; set; }
    public BuilderProfile BuilderProfile { get; set; } = null!;

    public string PlatformPostId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string PostUrl { get; set; } = string.Empty;
    public string PostType { get; set; } = "Tweet"; // Tweet, Thread, Demo, Announcement
    public string MediaUrlsJson { get; set; } = "[]";

    public ContentCategory? ClassifiedCategory { get; set; }
    public string KeywordsJson { get; set; } = "[]";

    // Current latest metric cache
    public int Likes { get; set; }
    public int Replies { get; set; }
    public int Reposts { get; set; }
    public int Views { get; set; }
    public int Bookmarks { get; set; }

    // Navigation
    public ICollection<PostMetricsSnapshot> MetricSnapshots { get; set; } = new List<PostMetricsSnapshot>();
    public ICollection<JourneyEventEvidence> EventEvidences { get; set; } = new List<JourneyEventEvidence>();
    public ICollection<InsightEvidence> InsightEvidences { get; set; } = new List<InsightEvidence>();
    public ICollection<ContentClassification> Classifications { get; set; } = new List<ContentClassification>();
}

public class PostMetricsSnapshot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContentPostId { get; set; }
    public ContentPost ContentPost { get; set; } = null!;

    public int Likes { get; set; }
    public int Replies { get; set; }
    public int Reposts { get; set; }
    public int Views { get; set; }
    public int Bookmarks { get; set; }
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}

public class AudienceMetricsSnapshot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuilderProfileId { get; set; }
    public BuilderProfile BuilderProfile { get; set; } = null!;

    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public int ListedCount { get; set; }
    public int TotalPostsCount { get; set; }
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}

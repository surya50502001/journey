using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Domain.Entities;

public class JourneyEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuilderProfileId { get; set; }
    public BuilderProfile BuilderProfile { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public DatePrecision Precision { get; set; } = DatePrecision.Exact;
    public JourneyEventCategory Category { get; set; }
    public ConfidenceLevel Confidence { get; set; } = ConfidenceLevel.High;
    public bool IsInferred { get; set; } = false; // False = explicitly stated by builder; True = algorithmic inference
    public string LimitationsNote { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<JourneyEventEvidence> Evidences { get; set; } = new List<JourneyEventEvidence>();
}

public class JourneyEventEvidence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JourneyEventId { get; set; }
    public JourneyEvent JourneyEvent { get; set; } = null!;

    public Guid ContentPostId { get; set; }
    public ContentPost ContentPost { get; set; } = null!;

    public EvidenceType EvidenceType { get; set; }
    public string Excerpt { get; set; } = string.Empty;
    public double Weight { get; set; } = 1.0;
}

public class ContentClassification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContentPostId { get; set; }
    public ContentPost ContentPost { get; set; } = null!;

    public ContentCategory PrimaryCategory { get; set; }
    public ContentCategory? SecondaryCategory { get; set; }
    public double ConfidenceScore { get; set; }
    public string IdentifiedKeywordsJson { get; set; } = "[]";
    public string Reasoning { get; set; } = string.Empty;
}

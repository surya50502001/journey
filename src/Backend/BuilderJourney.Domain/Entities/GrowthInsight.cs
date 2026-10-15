namespace BuilderJourney.Domain.Entities;

public class GrowthInsight
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuilderProfileId { get; set; }
    public BuilderProfile BuilderProfile { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string MetricObserved { get; set; } = string.Empty; // e.g. "Average replies per demo post"
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string MagnitudeChange { get; set; } = string.Empty; // e.g. "+312% increase"
    
    // Rigorous, non-causal disclosures:
    public string PlausibleExplanations { get; set; } = string.Empty; // "Coincided with introduction of video screen recordings"
    public string Limitations { get; set; } = string.Empty; // "Sample size is 8 posts during this window; causation is not proven."
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<InsightEvidence> Evidences { get; set; } = new List<InsightEvidence>();
}

public class InsightEvidence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GrowthInsightId { get; set; }
    public GrowthInsight GrowthInsight { get; set; } = null!;

    public Guid ContentPostId { get; set; }
    public ContentPost ContentPost { get; set; } = null!;

    public string Explanation { get; set; } = string.Empty;
}

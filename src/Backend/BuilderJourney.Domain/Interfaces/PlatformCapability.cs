using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Domain.Interfaces;

public class PlatformCapability
{
    public PlatformType Platform { get; set; }
    public bool SupportsLikes { get; set; }
    public bool SupportsReplies { get; set; }
    public bool SupportsReposts { get; set; }
    public bool SupportsViews { get; set; }
    public bool SupportsBookmarks { get; set; }
    public bool SupportsHistoricalFollowers { get; set; }
    public bool SupportsFullHistory { get; set; }
    public string RateLimitDescription { get; set; } = string.Empty;
    public string DataRetentionPolicy { get; set; } = string.Empty;
    public string AttributionRequirement { get; set; } = string.Empty;
    public IReadOnlyList<string> Limitations { get; set; } = new List<string>();
}

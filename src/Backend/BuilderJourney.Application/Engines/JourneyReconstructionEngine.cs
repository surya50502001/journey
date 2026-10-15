using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.Engines;

public interface IJourneyReconstructionEngine
{
    List<JourneyEvent> ReconstructJourney(BuilderProfile profile, IEnumerable<ContentPost> posts);
    List<string> GetReconstructionLimitations(BuilderProfile profile, int postCount, int eventCount);
}

public class JourneyReconstructionEngine : IJourneyReconstructionEngine
{
    private static readonly (string[] Keywords, JourneyEventCategory Category, string TitlePrefix, ConfidenceLevel Confidence, bool IsInferred)[] Patterns =
    [
        (
            new[] { "first paying customer", "first subscriber", "first sale", "stripe alert", "customer #1", "first dollar online", "first paid user" },
            JourneyEventCategory.FirstCustomer,
            "First Documented Customer",
            ConfidenceLevel.High,
            false
        ),
        (
            new[] { "launched on product hunt", "show hn", "we're live", "officially launched", "introducing", "now live for everyone", "public launch", "launched today" },
            JourneyEventCategory.PublicLaunch,
            "Public Product Launch",
            ConfidenceLevel.High,
            false
        ),
        (
            new[] { "idea for", "thinking of building", "starting to build", "day 1 of", "what if i built", "experimenting with", "building a new tool", "starting a new project" },
            JourneyEventCategory.InitialIdea,
            "Initial Idea & Concept Formulation",
            ConfidenceLevel.High,
            false
        ),
        (
            new[] { "quick demo", "screen recording", "loom demo", "early prototype", "v0.1", "alpha is ready", "working prototype", "first video demo" },
            JourneyEventCategory.PrototypeDemo,
            "First Documented Prototype Demo",
            ConfidenceLevel.High,
            false
        ),
        (
            new[] { "decided to pivot", "pivoting to", "killing the project", "shutting down", "moving away from", "rebranding to", "rebooting" },
            JourneyEventCategory.Pivot,
            "Strategic Pivot / Direction Shift",
            ConfidenceLevel.High,
            false
        ),
        (
            new[] { "what went wrong", "mistake i made", "post-mortem", "lesson learned", "failed launch", "why it failed", "hard lesson" },
            JourneyEventCategory.FailureOrLesson,
            "Documented Failure & Key Lesson",
            ConfidenceLevel.High,
            false
        ),
        (
            new[] { "talking to customers", "surveyed users", "user interviews", "validating the problem", "waitlist reached", "problem validation" },
            JourneyEventCategory.ProblemValidation,
            "Problem Validation & User Research",
            ConfidenceLevel.Medium,
            true
        ),
        (
            new[] { "shipped v2", "major update", "new feature dropped", "changelog", "based on your feedback", "version 2.0", "redesigned the app" },
            JourneyEventCategory.ProductIteration,
            "Major Product Iteration",
            ConfidenceLevel.Medium,
            false
        ),
        (
            new[] { "1,000 followers", "10k followers", "50k followers", "100k followers", "newsletter hit", "subscribers milestone", "milestone reached" },
            JourneyEventCategory.AudienceMilestone,
            "Audience Milestone Documented",
            ConfidenceLevel.High,
            false
        ),
        (
            new[] { "profitable", "ramen profitable", "$10k mrr", "$50k mrr", "$100k arr", "cash flow positive", "record revenue", "all-time high revenue" },
            JourneyEventCategory.BusinessMilestone,
            "Public Business Milestone",
            ConfidenceLevel.Medium,
            false
        ),
        (
            new[] { "traffic surge", "viral post", "exponential growth", "traffic exploded", "servers crashed from traffic" },
            JourneyEventCategory.Growth,
            "Growth Influx / Attention Spike",
            ConfidenceLevel.Medium,
            true
        )
    ];

    public List<JourneyEvent> ReconstructJourney(BuilderProfile profile, IEnumerable<ContentPost> posts)
    {
        var sortedPosts = posts.OrderBy(p => p.PublishedAt).ToList();
        var reconstructedEvents = new List<JourneyEvent>();

        if (sortedPosts.Count == 0)
        {
            return reconstructedEvents;
        }

        // 1. Pattern-based event detection
        foreach (var post in sortedPosts)
        {
            var contentLower = post.Content.ToLowerInvariant();

            foreach (var (keywords, category, titlePrefix, confidence, isInferred) in Patterns)
            {
                var matchedKeyword = keywords.FirstOrDefault(k => contentLower.Contains(k));
                if (matchedKeyword != null)
                {
                    // Avoid duplicate events of the exact same category on the exact same date
                    var existing = reconstructedEvents.FirstOrDefault(e =>
                        e.Category == category &&
                        Math.Abs((e.EventDate - post.PublishedAt).TotalDays) < 3);

                    if (existing == null)
                    {
                        var eventEntity = new JourneyEvent
                        {
                            Id = Guid.NewGuid(),
                            BuilderProfileId = profile.Id,
                            BuilderProfile = profile,
                            Title = $"{titlePrefix}: \"{TruncateText(post.Content, 50)}\"",
                            Description = GenerateEventDescription(category, post.Content, matchedKeyword, isInferred),
                            EventDate = post.PublishedAt,
                            Precision = DatePrecision.Exact,
                            Category = category,
                            Confidence = confidence,
                            IsInferred = isInferred,
                            LimitationsNote = isInferred
                                ? "Event context inferred from post semantics; no external verification beyond public author statement."
                                : "Directly stated in public builder statement.",
                            CreatedAt = DateTime.UtcNow
                        };

                        eventEntity.Evidences.Add(new JourneyEventEvidence
                        {
                            Id = Guid.NewGuid(),
                            JourneyEventId = eventEntity.Id,
                            JourneyEvent = eventEntity,
                            ContentPostId = post.Id,
                            ContentPost = post,
                            EvidenceType = isInferred ? EvidenceType.TemporalContext : EvidenceType.DirectStatement,
                            Excerpt = TruncateText(post.Content, 280),
                            Weight = isInferred ? 0.75 : 1.0
                        });

                        reconstructedEvents.Add(eventEntity);
                        break; // One primary event per post
                    }
                }
            }
        }

        // 2. Structural/Engagement Spike Inference (for unmentioned launches or breakthrough posts)
        var engagementThreshold = CalculateEngagementSpikeThreshold(sortedPosts);
        foreach (var post in sortedPosts)
        {
            var totalEngagement = post.Likes + post.Replies * 2 + post.Reposts * 3;
            if (totalEngagement >= engagementThreshold && totalEngagement > 100)
            {
                var isAlreadyDocumented = reconstructedEvents.Any(e => e.Evidences.Any(ev => ev.ContentPostId == post.Id));
                if (!isAlreadyDocumented)
                {
                    var eventEntity = new JourneyEvent
                    {
                        Id = Guid.NewGuid(),
                        BuilderProfileId = profile.Id,
                        BuilderProfile = profile,
                        Title = $"Breakthrough Resonance: \"{TruncateText(post.Content, 45)}\"",
                        Description = $"This post received {post.Likes:N0} likes, {post.Replies:N0} replies, and {post.Reposts:N0} reposts, representing a statistically significant resonance surge compared to typical posts.",
                        EventDate = post.PublishedAt,
                        Precision = DatePrecision.Exact,
                        Category = JourneyEventCategory.Growth,
                        Confidence = ConfidenceLevel.Medium,
                        IsInferred = true,
                        LimitationsNote = "Inferred from quantitative engagement spike; underlying product or business metrics cannot be verified from post metrics alone.",
                        CreatedAt = DateTime.UtcNow
                    };

                    eventEntity.Evidences.Add(new JourneyEventEvidence
                    {
                        Id = Guid.NewGuid(),
                        JourneyEventId = eventEntity.Id,
                        JourneyEvent = eventEntity,
                        ContentPostId = post.Id,
                        ContentPost = post,
                        EvidenceType = EvidenceType.MetricSurge,
                        Excerpt = TruncateText(post.Content, 280),
                        Weight = 0.85
                    });

                    reconstructedEvents.Add(eventEntity);
                }
            }
        }

        return reconstructedEvents.OrderBy(e => e.EventDate).ToList();
    }

    public List<string> GetReconstructionLimitations(BuilderProfile profile, int postCount, int eventCount)
    {
        var limitations = new List<string>
        {
            "Reconstruction is strictly limited to publicly available, authorized social posts and verifiable timestamps.",
            "Private metrics (exact Stripe revenue, churn, user retention, internal team discussions) cannot be verified.",
            "Historical post deletions, unposted offline pivots, or algorithm changes by the social platform may omit parts of the timeline.",
            "Inferred events represent algorithmic pattern matching and should be verified against primary sources."
        };

        if (postCount < 20)
        {
            limitations.Add("Limited sample size: fewer than 20 posts analyzed. Historical milestones may be incomplete.");
        }

        return limitations;
    }

    private static double CalculateEngagementSpikeThreshold(List<ContentPost> posts)
    {
        if (posts.Count < 5) return 200;

        var scores = posts.Select(p => (double)(p.Likes + p.Replies * 2 + p.Reposts * 3)).OrderBy(s => s).ToList();
        var p90Index = (int)(scores.Count * 0.90);
        return Math.Max(150, scores[p90Index]);
    }

    private static string GenerateEventDescription(JourneyEventCategory category, string content, string keyword, bool isInferred)
    {
        var sanitized = TruncateText(content, 140);
        return category switch
        {
            JourneyEventCategory.InitialIdea => $"Builder publicly formulated the initial concept and shared early thoughts: \"{sanitized}\"",
            JourneyEventCategory.ProblemValidation => $"Builder engaged in user discovery or problem validation discussions: \"{sanitized}\"",
            JourneyEventCategory.PrototypeDemo => $"Builder showcased an early prototype, interactive screen capture, or demo: \"{sanitized}\"",
            JourneyEventCategory.PublicLaunch => $"Builder announced the public launch of their product or milestone version: \"{sanitized}\"",
            JourneyEventCategory.FirstCustomer => $"Builder documented their first paying user or initial transaction milestone: \"{sanitized}\"",
            JourneyEventCategory.ProductIteration => $"Builder released a major iteration, feature update, or version upgrade: \"{sanitized}\"",
            JourneyEventCategory.Pivot => $"Builder announced a strategic pivot, sunsetting, or major redirection: \"{sanitized}\"",
            JourneyEventCategory.AudienceMilestone => $"Builder reached and publicly celebrated an audience or subscriber milestone: \"{sanitized}\"",
            JourneyEventCategory.BusinessMilestone => $"Builder reported a public business revenue or operational milestone: \"{sanitized}\"",
            JourneyEventCategory.FailureOrLesson => $"Builder shared a retrospective on a challenge, failure, or key takeaway: \"{sanitized}\"",
            JourneyEventCategory.Growth => $"Builder documented a period of rapid traction, attention surge, or adoption: \"{sanitized}\"",
            _ => $"Documented builder journey event matching keyword '{keyword}': \"{sanitized}\""
        };
    }

    private static string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        var clean = text.Replace("\r", " ").Replace("\n", " ").Trim();
        return clean.Length <= maxLength ? clean : clean[..maxLength] + "...";
    }
}

using BuilderJourney.Application.DTOs;
using BuilderJourney.Domain.Entities;

namespace BuilderJourney.Application.Engines;

public interface IBreakthroughDetector
{
    List<BreakthroughDto> DetectBreakthroughs(BuilderProfile profile, IReadOnlyList<ContentPost> posts);
}

public class BreakthroughDetector : IBreakthroughDetector
{
    public List<BreakthroughDto> DetectBreakthroughs(BuilderProfile profile, IReadOnlyList<ContentPost> posts)
    {
        var breakthroughs = new List<BreakthroughDto>();
        var sortedPosts = posts.OrderBy(p => p.PublishedAt).ToList();

        if (sortedPosts.Count < 6)
        {
            return breakthroughs;
        }

        // 1. Detect Major Launch / Viral Inflection Points
        var baselineAvg = sortedPosts.Average(p => p.Likes);
        var highSpikePosts = sortedPosts
            .Where(p => p.Likes >= Math.Max(100, baselineAvg * 3.0))
            .OrderByDescending(p => p.Likes)
            .Take(3)
            .ToList();

        foreach (var spikePost in highSpikePosts)
        {
            var windowStart = spikePost.PublishedAt.AddDays(-14);
            var windowEnd = spikePost.PublishedAt.AddDays(14);

            var beforePosts = sortedPosts
                .Where(p => p.PublishedAt < spikePost.PublishedAt && p.PublishedAt >= windowStart)
                .TakeLast(2)
                .ToList();

            var afterPosts = sortedPosts
                .Where(p => p.PublishedAt > spikePost.PublishedAt && p.PublishedAt <= windowEnd)
                .Take(2)
                .ToList();

            var beforeAvgLikes = beforePosts.Count > 0 ? beforePosts.Average(p => p.Likes) : baselineAvg;
            var multiplier = Math.Round(spikePost.Likes / Math.Max(1.0, beforeAvgLikes), 1);

            var evidences = new List<BreakthroughEvidenceDto>();
            foreach (var bp in beforePosts)
            {
                evidences.Add(new BreakthroughEvidenceDto(
                    bp.Id,
                    bp.Content,
                    bp.PublishedAt,
                    bp.PostUrl,
                    bp.Likes,
                    bp.Replies,
                    "Pre-inflection baseline post"
                ));
            }

            evidences.Add(new BreakthroughEvidenceDto(
                spikePost.Id,
                spikePost.Content,
                spikePost.PublishedAt,
                spikePost.PostUrl,
                spikePost.Likes,
                spikePost.Replies,
                "Key inflection milestone post"
            ));

            foreach (var ap in afterPosts)
            {
                evidences.Add(new BreakthroughEvidenceDto(
                    ap.Id,
                    ap.Content,
                    ap.PublishedAt,
                    ap.PostUrl,
                    ap.Likes,
                    ap.Replies,
                    "Post-inflection follow-up activity"
                ));
            }

            var title = spikePost.Content.Contains("launch", StringComparison.OrdinalIgnoreCase)
                ? $"Launch Surge Inflection: {multiplier}x Engagement Increase"
                : $"Viral Resonance Inflection: {multiplier}x Engagement Increase";

            breakthroughs.Add(new BreakthroughDto(
                Guid.NewGuid(),
                title,
                $"On {spikePost.PublishedAt:MMM dd, yyyy}, a public post generated {spikePost.Likes:N0} likes and {spikePost.Replies:N0} replies ({multiplier}x higher than the pre-period baseline).",
                $"Likes increased from ~{beforeAvgLikes:N0} to {spikePost.Likes:N0}",
                windowStart,
                windowEnd,
                $"+{multiplier * 100:N0}% spike",
                "The post coincided with a concise product demonstration or public milestone announcement that resonated strongly with developer and builder communities.",
                "Limitations: Causation cannot be established. Platform algorithm distribution, trending topics, or external retweets may have contributed to this visibility.",
                evidences
            ));
        }

        // 2. Detect Format Shift (e.g., Text updates vs Video/Demo introductions)
        var demoPosts = sortedPosts.Where(p => p.ClassifiedCategory == Domain.Enums.ContentCategory.ProductDemo).ToList();
        var textPosts = sortedPosts.Where(p => p.ClassifiedCategory == Domain.Enums.ContentCategory.BuildInPublic).ToList();

        if (demoPosts.Count >= 2 && textPosts.Count >= 2)
        {
            var avgDemoReplies = demoPosts.Average(p => p.Replies);
            var avgTextReplies = textPosts.Average(p => p.Replies);

            if (avgDemoReplies >= avgTextReplies * 1.5 && avgDemoReplies > 5)
            {
                var diffPct = Math.Round((avgDemoReplies - avgTextReplies) / Math.Max(1.0, avgTextReplies) * 100, 0);
                var sampleDemo = demoPosts.OrderByDescending(p => p.Replies).First();
                var sampleText = textPosts.OrderByDescending(p => p.Replies).First();

                breakthroughs.Add(new BreakthroughDto(
                    Guid.NewGuid(),
                    "Format Shift: Product Demos Drive Superior Discussion Depth",
                    $"Posts featuring live product demonstrations or visual walkthroughs generated an average of {avgDemoReplies:N1} replies versus {avgTextReplies:N1} replies for text-only updates (+{diffPct}% deeper discussion).",
                    "Reply Volume & Community Discussion Depth",
                    demoPosts.Min(p => p.PublishedAt),
                    demoPosts.Max(p => p.PublishedAt),
                    $"+{diffPct}% discussion depth",
                    "Demonstrating real software working in video format is associated with substantially more specific technical feedback and user questions than generic progress updates.",
                    "Alternative explanations: Video posts may be shared more selectively at later stages of product maturity when audience curiosity is naturally higher.",
                    new List<BreakthroughEvidenceDto>
                    {
                        new(sampleText.Id, sampleText.Content, sampleText.PublishedAt, sampleText.PostUrl, sampleText.Likes, sampleText.Replies, "Typical text-only update"),
                        new(sampleDemo.Id, sampleDemo.Content, sampleDemo.PublishedAt, sampleDemo.PostUrl, sampleDemo.Likes, sampleDemo.Replies, "High-discussion visual demo")
                    }
                ));
            }
        }

        return breakthroughs;
    }
}

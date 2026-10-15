using BuilderJourney.Application.DTOs;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.Engines;

public interface IBuilderComparator
{
    BuilderComparisonResultDto CompareBuilders(
        IReadOnlyList<BuilderProfile> builders,
        IContentStrategyAnalyzer contentAnalyzer);
}

public class BuilderComparator : IBuilderComparator
{
    public BuilderComparisonResultDto CompareBuilders(
        IReadOnlyList<BuilderProfile> builders,
        IContentStrategyAnalyzer contentAnalyzer)
    {
        if (builders.Count == 0)
        {
            return new BuilderComparisonResultDto(
                new List<BuilderComparisonProfileDto>(),
                new List<BuilderComparisonMetric>(),
                new Dictionary<Guid, List<CategoryDistributionDto>>(),
                new Dictionary<Guid, List<JourneyEventDto>>(),
                "No builders selected for comparison.",
                "Analytical data is unavailable."
            );
        }

        var profileDtos = new List<BuilderComparisonProfileDto>();
        var categoryMixByBuilder = new Dictionary<Guid, List<CategoryDistributionDto>>();
        var alignedMilestonesByBuilder = new Dictionary<Guid, List<JourneyEventDto>>();

        foreach (var b in builders)
        {
            var analysis = contentAnalyzer.AnalyzeStrategy(b, b.Posts.ToList());
            var primaryFormat = analysis.FormatPerformances.OrderByDescending(f => f.Count).FirstOrDefault()?.FormatName ?? "Standard Posts";
            categoryMixByBuilder[b.Id] = analysis.Categories;

            var events = b.JourneyEvents.OrderBy(e => e.EventDate).Select(e => new JourneyEventDto(
                e.Id,
                e.Title,
                e.Description,
                e.EventDate,
                e.Precision,
                e.Category,
                e.Category.ToString(),
                e.Confidence,
                e.IsInferred,
                e.LimitationsNote,
                e.Evidences.Select(ev => new JourneyEventEvidenceDto(
                    ev.Id,
                    ev.ContentPostId,
                    ev.ContentPost?.Content ?? "",
                    ev.ContentPost?.PublishedAt ?? e.EventDate,
                    ev.ContentPost?.PostUrl ?? "",
                    ev.ContentPost?.PostType ?? "Tweet",
                    ev.ContentPost?.Likes ?? 0,
                    ev.ContentPost?.Replies ?? 0,
                    ev.ContentPost?.Reposts ?? 0,
                    ev.EvidenceType,
                    ev.Excerpt,
                    ev.Weight
                )).ToList()
            )).ToList();

            alignedMilestonesByBuilder[b.Id] = events;

            profileDtos.Add(new BuilderComparisonProfileDto(
                b.Id,
                b.Name,
                b.Handle,
                b.AvatarUrl,
                b.BuilderCategory?.Name ?? "SaaS",
                b.FollowersCount,
                b.Posts.Count,
                b.JourneyEvents.Count,
                b.DocumentedStartDate,
                primaryFormat,
                $"{analysis.AveragePostsPerWeek} posts/week"
            ));
        }

        // Build comparative metrics table
        var comparativeMetrics = new List<BuilderComparisonMetric>
        {
            new(
                "Documented Starting Point",
                builders.ToDictionary(b => b.Id, b => b.DocumentedStartDate?.ToString("MMM yyyy") ?? "Earliest Post"),
                "First observed build-in-public activity or launch record."
            ),
            new(
                "Audience Scale (Public Followers)",
                builders.ToDictionary(b => b.Id, b => $"{b.FollowersCount:N0}"),
                "Latest public follower count observed on platform."
            ),
            new(
                "Posting Cadence",
                builders.ToDictionary(b => b.Id, b => {
                    var strategy = contentAnalyzer.AnalyzeStrategy(b, b.Posts.ToList());
                    return $"{strategy.AveragePostsPerWeek} / week ({strategy.ConsistencyScore})";
                }),
                "Average publishing frequency over the documented active timeframe."
            ),
            new(
                "Documented Milestones",
                builders.ToDictionary(b => b.Id, b => $"{b.JourneyEvents.Count} milestones"),
                "Verifiable launch, customer, pivot, or growth events identified."
            ),
            new(
                "Top Resonance Format",
                builders.ToDictionary(b => b.Id, b => {
                    var strategy = contentAnalyzer.AnalyzeStrategy(b, b.Posts.ToList());
                    var top = strategy.FormatPerformances.OrderByDescending(f => f.EngagementMultiplier).FirstOrDefault();
                    return top != null ? $"{top.FormatName} ({top.EngagementMultiplier}x baseline)" : "Text updates";
                }),
                "Format exhibiting the highest relative engagement multiplier."
            ),
            new(
                "Average Likes per Post",
                builders.ToDictionary(b => b.Id, b => b.Posts.Count > 0 ? $"{b.Posts.Average(p => p.Likes):N1}" : "N/A"),
                "Arithmetic mean of likes across all documented posts."
            ),
            new(
                "Average Discussion Depth (Replies)",
                builders.ToDictionary(b => b.Id, b => b.Posts.Count > 0 ? $"{b.Posts.Average(p => p.Replies):N1}" : "N/A"),
                "Direct measure of community conversation per post."
            )
        };

        // Synthesize comparative narrative
        var first = profileDtos.FirstOrDefault();
        var second = profileDtos.Skip(1).FirstOrDefault();
        var synthesis = second != null
            ? $"Comparing {first?.Name} and {second.Name} reveals distinct operational playbooks. While {first?.Name} focuses on {first?.PrimaryContentStyle} at {first?.LaunchPacing}, {second.Name} exhibits a different focus on {second.PrimaryContentStyle} at {second.LaunchPacing}. Both demonstrate consistent documented cadence preceding major launch milestones."
            : "Comparative analysis requires two or more builder profiles.";

        var notice = "Analytical Limitation: Engagement disparities do not reflect software reliability, unit economics, or true profitability. Cross-builder comparisons are descriptive, not causal.";

        return new BuilderComparisonResultDto(
            profileDtos,
            comparativeMetrics,
            categoryMixByBuilder,
            alignedMilestonesByBuilder,
            synthesis,
            notice
        );
    }
}

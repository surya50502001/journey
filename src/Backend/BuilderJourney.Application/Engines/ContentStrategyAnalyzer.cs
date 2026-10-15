using System.Text.RegularExpressions;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.Engines;

public interface IContentStrategyAnalyzer
{
    ContentCategory ClassifyPost(ContentPost post);
    ContentStrategySummaryDto AnalyzeStrategy(BuilderProfile profile, IReadOnlyList<ContentPost> posts);
}

public class ContentStrategyAnalyzer : IContentStrategyAnalyzer
{
    private static readonly (ContentCategory Category, string[] Keywords, double Weight)[] CategoryRules =
    [
        (
            ContentCategory.ProductDemo,
            new[] { "demo", "loom", "video", "preview", "screen recording", "watch how", "interactive walkthrough", "ui preview" },
            1.5
        ),
        (
            ContentCategory.EducationalThread,
            new[] { "thread", "1/", "🧵", "here is a breakdown", "step by step breakdown", "how we scaled", "how i built" },
            1.4
        ),
        (
            ContentCategory.LaunchAnnouncement,
            new[] { "launched", "we're live", "now live", "product hunt", "show hn", "introducing", "public release", "v1.0 is live" },
            1.4
        ),
        (
            ContentCategory.LessonsLearned,
            new[] { "lesson learned", "mistake i made", "post-mortem", "what went wrong", "what i wish i knew", "key takeaways", "hard truth" },
            1.3
        ),
        (
            ContentCategory.CustomerFeedback,
            new[] { "customer said", "user feedback", "testimonial", "dm from a user", "customer review", "shoutout to our user" },
            1.2
        ),
        (
            ContentCategory.PersonalStory,
            new[] { "my journey", "quit my job", "when i started", "years ago", "life update", "bootstrapping journey", "digital nomad" },
            1.1
        ),
        (
            ContentCategory.TechnicalTutorial,
            new[] { "tutorial", "how to", "architecture", "code snippet", "guide", "tech stack", "database schema", "performance optimization" },
            1.2
        ),
        (
            ContentCategory.Promotional,
            new[] { "discount", "black friday", "lifetime deal", "special offer", "pricing plans", "coupon code", "buy now", "early bird" },
            1.1
        ),
        (
            ContentCategory.DiscussionQuestion,
            new[] { "what do you think", "how do you handle", "question for builders", "poll", "curious how you", "which do you prefer" },
            1.0
        ),
        (
            ContentCategory.BuildInPublic,
            new[] { "buildinpublic", "#buildinpublic", "indiehackers", "today's update", "shipping today", "changelog", "daily progress" },
            1.0
        )
    ];

    public ContentCategory ClassifyPost(ContentPost post)
    {
        var text = (post.Content ?? string.Empty).ToLowerInvariant();
        var bestCategory = ContentCategory.BuildInPublic; // Default baseline for builder accounts
        var highestScore = 0.0;

        // Check media signals
        var hasMedia = !string.IsNullOrWhiteSpace(post.MediaUrlsJson) && post.MediaUrlsJson != "[]";
        if (hasMedia && (text.Contains("demo") || text.Contains("look") || text.Contains("preview") || text.Contains("ui")))
        {
            highestScore += 2.0;
            bestCategory = ContentCategory.ProductDemo;
        }

        // Check thread indicator
        if (text.StartsWith("1/") || text.Contains("🧵") || text.Contains("a thread"))
        {
            var threadScore = 2.5;
            if (threadScore > highestScore)
            {
                highestScore = threadScore;
                bestCategory = ContentCategory.EducationalThread;
            }
        }

        // Check question mark
        if (text.Contains("?") && (text.Contains("you") || text.Contains("builders") || text.Contains("opinion")))
        {
            var questionScore = 1.8;
            if (questionScore > highestScore)
            {
                highestScore = questionScore;
                bestCategory = ContentCategory.DiscussionQuestion;
            }
        }

        foreach (var (category, keywords, weight) in CategoryRules)
        {
            var matchCount = keywords.Count(k => text.Contains(k));
            var score = matchCount * weight;
            if (score > highestScore)
            {
                highestScore = score;
                bestCategory = category;
            }
        }

        return bestCategory;
    }

    public ContentStrategySummaryDto AnalyzeStrategy(BuilderProfile profile, IReadOnlyList<ContentPost> posts)
    {
        if (posts.Count == 0)
        {
            return new ContentStrategySummaryDto(
                profile.Id,
                profile.Name,
                profile.Handle,
                0,
                null,
                null,
                0,
                "Insufficient Data",
                new List<CategoryDistributionDto>(),
                new List<PostingCadenceDto>(),
                new List<FormatPerformanceDto>(),
                new List<TopTopicDto>(),
                new List<PostSummaryDto>(),
                "No posts have been ingested for this builder yet.",
                "Data is unavailable for content analysis."
            );
        }

        var sortedPosts = posts.OrderBy(p => p.PublishedAt).ToList();
        var earliest = sortedPosts.First().PublishedAt;
        var latest = sortedPosts.Last().PublishedAt;
        var totalWeeks = Math.Max(1.0, (latest - earliest).TotalDays / 7.0);
        var postsPerWeek = Math.Round(sortedPosts.Count / totalWeeks, 1);

        // Classify each post
        var classifiedPosts = sortedPosts.Select(p => new
        {
            Post = p,
            Category = p.ClassifiedCategory ?? ClassifyPost(p)
        }).ToList();

        // Category distribution
        var categoryDistributions = Enum.GetValues<ContentCategory>()
            .Select(cat =>
            {
                var group = classifiedPosts.Where(cp => cp.Category == cat).Select(cp => cp.Post).ToList();
                var count = group.Count;
                var pct = Math.Round((double)count / sortedPosts.Count * 100, 1);
                var avgLikes = count > 0 ? Math.Round(group.Average(p => p.Likes), 1) : 0;
                var avgReplies = count > 0 ? Math.Round(group.Average(p => p.Replies), 1) : 0;
                var avgReposts = count > 0 ? Math.Round(group.Average(p => p.Reposts), 1) : 0;

                return new CategoryDistributionDto(
                    cat,
                    FormatCategoryName(cat),
                    count,
                    pct,
                    avgLikes,
                    avgReplies,
                    avgReposts
                );
            })
            .Where(d => d.PostCount > 0)
            .OrderByDescending(d => d.PostCount)
            .ToList();

        // Cadence History (group by Quarter or Month)
        var cadenceHistory = sortedPosts
            .GroupBy(p => $"{p.PublishedAt.Year}-Q{((p.PublishedAt.Month - 1) / 3) + 1}")
            .OrderBy(g => g.Key)
            .Select(g => new PostingCadenceDto(
                g.Key,
                g.Count(),
                Math.Round(g.Average(p => p.Likes), 1),
                Math.Round(g.Average(p => p.Replies), 1)
            ))
            .ToList();

        // Consistency score
        var cadenceVariance = CalculateCadenceVariance(cadenceHistory.Select(c => c.PostCount).ToList());
        var consistencyScore = cadenceVariance < 0.35 ? "High (Consistent rhythm)" :
                               cadenceVariance < 0.70 ? "Moderate (Intermittent shipping sprints)" :
                               "Variable (Bursts around launches and lulls)";

        // Format performance
        var baselineEngagement = sortedPosts.Average(p => p.Likes + p.Replies * 2);
        if (baselineEngagement <= 0) baselineEngagement = 1.0;

        var formatPerformances = new List<FormatPerformanceDto>();
        var demoPosts = classifiedPosts.Where(cp => cp.Category == ContentCategory.ProductDemo).Select(cp => cp.Post).ToList();
        if (demoPosts.Count > 0)
        {
            var demoEng = demoPosts.Average(p => p.Likes + p.Replies * 2);
            formatPerformances.Add(new FormatPerformanceDto(
                "Product Demos & Videos",
                demoPosts.Count,
                Math.Round(demoPosts.Average(p => p.Likes), 1),
                Math.Round(demoPosts.Average(p => p.Replies), 1),
                Math.Round(demoPosts.Average(p => p.Reposts), 1),
                Math.Round(demoEng / baselineEngagement, 2)
            ));
        }

        var threadPosts = classifiedPosts.Where(cp => cp.Category == ContentCategory.EducationalThread).Select(cp => cp.Post).ToList();
        if (threadPosts.Count > 0)
        {
            var threadEng = threadPosts.Average(p => p.Likes + p.Replies * 2);
            formatPerformances.Add(new FormatPerformanceDto(
                "Educational Threads",
                threadPosts.Count,
                Math.Round(threadPosts.Average(p => p.Likes), 1),
                Math.Round(threadPosts.Average(p => p.Replies), 1),
                Math.Round(threadPosts.Average(p => p.Reposts), 1),
                Math.Round(threadEng / baselineEngagement, 2)
            ));
        }

        var bipPosts = classifiedPosts.Where(cp => cp.Category == ContentCategory.BuildInPublic).Select(cp => cp.Post).ToList();
        if (bipPosts.Count > 0)
        {
            var bipEng = bipPosts.Average(p => p.Likes + p.Replies * 2);
            formatPerformances.Add(new FormatPerformanceDto(
                "Build-in-Public Micro-Updates",
                bipPosts.Count,
                Math.Round(bipPosts.Average(p => p.Likes), 1),
                Math.Round(bipPosts.Average(p => p.Replies), 1),
                Math.Round(bipPosts.Average(p => p.Reposts), 1),
                Math.Round(bipEng / baselineEngagement, 2)
            ));
        }

        var questionPosts = classifiedPosts.Where(cp => cp.Category == ContentCategory.DiscussionQuestion).Select(cp => cp.Post).ToList();
        if (questionPosts.Count > 0)
        {
            var qEng = questionPosts.Average(p => p.Likes + p.Replies * 2);
            formatPerformances.Add(new FormatPerformanceDto(
                "Discussion Prompts & Questions",
                questionPosts.Count,
                Math.Round(questionPosts.Average(p => p.Likes), 1),
                Math.Round(questionPosts.Average(p => p.Replies), 1),
                Math.Round(questionPosts.Average(p => p.Reposts), 1),
                Math.Round(qEng / baselineEngagement, 2)
            ));
        }

        // Top topics
        var topTopics = ExtractTopTopics(sortedPosts);

        // Top resonance posts (Z-score or engagement rank)
        var topResonancePosts = sortedPosts
            .OrderByDescending(p => p.Likes + p.Replies * 2 + p.Reposts * 3)
            .Take(5)
            .Select(p => new PostSummaryDto(
                p.Id,
                p.PlatformPostId,
                p.Content,
                p.PublishedAt,
                p.PostUrl,
                p.PostType,
                p.ClassifiedCategory ?? ClassifyPost(p),
                p.Likes,
                p.Replies,
                p.Reposts,
                p.Views,
                p.Bookmarks
            ))
            .ToList();

        // Overview narrative
        var dominantCategory = categoryDistributions.FirstOrDefault()?.CategoryName ?? "Build-in-Public";
        var topFormat = formatPerformances.OrderByDescending(f => f.EngagementMultiplier).FirstOrDefault()?.FormatName ?? "Product Demos";
        var overview = $"{profile.Name} publishes an average of {postsPerWeek} posts per week with a {consistencyScore.ToLowerInvariant()}. " +
                       $"Their content strategy is predominantly weighted toward {dominantCategory} ({categoryDistributions.FirstOrDefault()?.Percentage}%), " +
                       $"while {topFormat} exhibits the highest relative audience resonance multiplier.";

        var notice = "Analytical Note: Higher social engagement (likes/replies) reflects audience interest and distribution resonance; it must not be conflated with customer conversion, software quality, or actual revenue.";

        return new ContentStrategySummaryDto(
            profile.Id,
            profile.Name,
            profile.Handle,
            sortedPosts.Count,
            earliest,
            latest,
            postsPerWeek,
            consistencyScore,
            categoryDistributions,
            cadenceHistory,
            formatPerformances,
            topTopics,
            topResonancePosts,
            overview,
            notice
        );
    }

    private static string FormatCategoryName(ContentCategory category) => category switch
    {
        ContentCategory.BuildInPublic => "Build-in-Public Updates",
        ContentCategory.ProductDemo => "Product Demos",
        ContentCategory.TechnicalTutorial => "Technical Tutorials",
        ContentCategory.EducationalThread => "Educational Threads",
        ContentCategory.PersonalStory => "Personal Stories & Journey",
        ContentCategory.LaunchAnnouncement => "Launch Announcements",
        ContentCategory.CustomerFeedback => "Customer Feedback",
        ContentCategory.LessonsLearned => "Lessons Learned & Post-Mortems",
        ContentCategory.DiscussionQuestion => "Questions & Discussions",
        ContentCategory.Promotional => "Promotional & Deals",
        _ => category.ToString()
    };

    private static double CalculateCadenceVariance(List<int> counts)
    {
        if (counts.Count < 2) return 0;
        var avg = counts.Average();
        if (avg <= 0) return 0;
        var sumSquares = counts.Sum(c => Math.Pow(c - avg, 2));
        var stdDev = Math.Sqrt(sumSquares / counts.Count);
        return stdDev / avg; // Coefficient of variation
    }

    private static List<TopTopicDto> ExtractTopTopics(List<ContentPost> posts)
    {
        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "the", "and", "this", "that", "with", "from", "for", "have", "been", "will", "what", "just", "about", "your", "they", "here", "were", "into", "some", "like", "more"
        };

        var wordCounts = new Dictionary<string, (int Count, double TotalEng, List<string> Samples)>(StringComparer.OrdinalIgnoreCase);

        foreach (var post in posts)
        {
            var words = Regex.Matches(post.Content, @"#?\b[A-Za-z]{4,15}\b")
                .Select(m => m.Value.ToLowerInvariant())
                .Where(w => !stopWords.Contains(w))
                .Distinct();

            var eng = post.Likes + post.Replies * 2;

            foreach (var word in words)
            {
                if (!wordCounts.TryGetValue(word, out var val))
                {
                    val = (0, 0, new List<string>());
                }

                if (val.Samples.Count < 2)
                {
                    val.Samples.Add(post.Content.Length > 80 ? post.Content[..80] + "..." : post.Content);
                }

                wordCounts[word] = (val.Count + 1, val.TotalEng + eng, val.Samples);
            }
        }

        return wordCounts
            .Where(kvp => kvp.Value.Count >= 2)
            .OrderByDescending(kvp => kvp.Value.Count)
            .Take(5)
            .Select(kvp => new TopTopicDto(
                kvp.Key.StartsWith("#") ? kvp.Key : "#" + kvp.Key,
                kvp.Value.Count,
                Math.Round(kvp.Value.TotalEng / Math.Max(1, kvp.Value.Count), 1),
                kvp.Value.Samples
            ))
            .ToList();
    }
}

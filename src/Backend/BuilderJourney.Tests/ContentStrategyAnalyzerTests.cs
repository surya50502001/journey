using BuilderJourney.Application.Engines;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace BuilderJourney.Tests;

public class ContentStrategyAnalyzerTests
{
    private readonly ContentStrategyAnalyzer _analyzer = new();

    [Fact]
    public void ClassifyPost_ShouldIdentifyProductDemo_WhenVideoOrDemoKeywordsPresent()
    {
        var post = new ContentPost
        {
            Content = "Here is a quick loom demo showing the interactive dashboard UI.",
            MediaUrlsJson = "[\"https://cdn.example.com/demo.mp4\"]"
        };

        var category = _analyzer.ClassifyPost(post);
        category.Should().Be(ContentCategory.ProductDemo);
    }

    [Fact]
    public void ClassifyPost_ShouldIdentifyEducationalThread_WhenNumberedOrThreadMarker()
    {
        var post = new ContentPost
        {
            Content = "1/ How we scaled our Postgres database to 10M rows on a $20 VPS without timeouts. A thread 🧵"
        };

        var category = _analyzer.ClassifyPost(post);
        category.Should().Be(ContentCategory.EducationalThread);
    }

    [Fact]
    public void AnalyzeStrategy_ShouldCalculateCadenceAndFormatMultipliers()
    {
        var builder = new BuilderProfile { Id = Guid.NewGuid(), Name = "Builder Pro", Handle = "@builder" };
        var posts = new List<ContentPost>
        {
            new() { PublishedAt = new DateTime(2023, 1, 1), Content = "Normal #buildinpublic update", Likes = 10, Replies = 2, ClassifiedCategory = ContentCategory.BuildInPublic },
            new() { PublishedAt = new DateTime(2023, 1, 8), Content = "Normal #buildinpublic update 2", Likes = 12, Replies = 2, ClassifiedCategory = ContentCategory.BuildInPublic },
            new() { PublishedAt = new DateTime(2023, 1, 15), Content = "Interactive video demo of the new search", Likes = 60, Replies = 25, ClassifiedCategory = ContentCategory.ProductDemo }
        };

        var summary = _analyzer.AnalyzeStrategy(builder, posts);

        summary.TotalAnalyzedPosts.Should().Be(3);
        summary.Categories.Should().NotBeEmpty();
        summary.FormatPerformances.Should().Contain(f => f.FormatName.Contains("Product Demos"));
        
        var demoFormat = summary.FormatPerformances.First(f => f.FormatName.Contains("Product Demos"));
        demoFormat.EngagementMultiplier.Should().BeGreaterThan(1.0);
        summary.LimitationsNotice.Should().Contain("must not be conflated with customer conversion");
    }
}

public class BreakthroughDetectorTests
{
    private readonly BreakthroughDetector _detector = new();

    [Fact]
    public void DetectBreakthroughs_ShouldDetectSignificantEngagementSurge()
    {
        var builder = new BuilderProfile { Id = Guid.NewGuid(), Name = "Builder", Handle = "@builder" };
        var posts = new List<ContentPost>();

        // 5 baseline posts with ~20 likes
        for (int i = 0; i < 5; i++)
        {
            posts.Add(new ContentPost
            {
                Id = Guid.NewGuid(),
                BuilderProfileId = builder.Id,
                PublishedAt = new DateTime(2023, 2, 1).AddDays(i * 3),
                Content = $"Regular update {i}",
                Likes = 20,
                Replies = 3
            });
        }

        // Inflection post with 350 likes (17x surge)
        posts.Add(new ContentPost
        {
            Id = Guid.NewGuid(),
            BuilderProfileId = builder.Id,
            PublishedAt = new DateTime(2023, 2, 20),
            Content = "Launched on Product Hunt! Live now with complete public demo.",
            Likes = 350,
            Replies = 65
        });

        var breakthroughs = _detector.DetectBreakthroughs(builder, posts);

        breakthroughs.Should().NotBeEmpty();
        var bt = breakthroughs.First();
        bt.Title.Should().Contain("Inflection");
        bt.Evidences.Should().HaveCountGreaterOrEqualTo(2);
        bt.LimitationsAndAlternatives.Should().Contain("Causation cannot be established");
    }
}

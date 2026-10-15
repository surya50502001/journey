using BuilderJourney.Application.Engines;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace BuilderJourney.Tests;

public class JourneyReconstructionEngineTests
{
    private readonly JourneyReconstructionEngine _engine = new();

    [Fact]
    public void ReconstructJourney_ShouldDetectPublicLaunch_WithHighConfidence()
    {
        // Arrange
        var builder = new BuilderProfile
        {
            Id = Guid.NewGuid(),
            Handle = "@indiedev",
            Name = "Indie Dev"
        };

        var posts = new List<ContentPost>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BuilderProfileId = builder.Id,
                Content = "Just officially launched on Product Hunt! We're live today with our v1.0 release.",
                PublishedAt = new DateTime(2023, 5, 10, 10, 0, 0, DateTimeKind.Utc),
                PostUrl = "https://x.com/indiedev/status/1",
                Likes = 150,
                Replies = 35,
                Reposts = 20
            }
        };

        // Act
        var events = _engine.ReconstructJourney(builder, posts);

        // Assert
        events.Should().HaveCount(1);
        var launchEvent = events.First();
        launchEvent.Category.Should().Be(JourneyEventCategory.PublicLaunch);
        launchEvent.Confidence.Should().Be(ConfidenceLevel.High);
        launchEvent.IsInferred.Should().BeFalse();
        launchEvent.Evidences.Should().HaveCount(1);
        launchEvent.Evidences.First().EvidenceType.Should().Be(EvidenceType.DirectStatement);
    }

    [Fact]
    public void ReconstructJourney_ShouldDetectFirstCustomer_AndBindEvidence()
    {
        // Arrange
        var builder = new BuilderProfile { Id = Guid.NewGuid(), Handle = "@solo", Name = "Solo Founder" };
        var post = new ContentPost
        {
            Id = Guid.NewGuid(),
            BuilderProfileId = builder.Id,
            Content = "Just got our first paying customer! $29 subscription recorded in Stripe. Huge milestone!",
            PublishedAt = new DateTime(2023, 6, 1, 14, 0, 0, DateTimeKind.Utc),
            Likes = 210,
            Replies = 42,
            Reposts = 15
        };

        // Act
        var events = _engine.ReconstructJourney(builder, new[] { post });

        // Assert
        events.Should().ContainSingle();
        var custEvent = events.Single();
        custEvent.Category.Should().Be(JourneyEventCategory.FirstCustomer);
        custEvent.Confidence.Should().Be(ConfidenceLevel.High);
        custEvent.Description.Should().Contain("first paying user");
    }

    [Fact]
    public void ReconstructJourney_ShouldDetectPivotAndRetrospective()
    {
        // Arrange
        var builder = new BuilderProfile { Id = Guid.NewGuid(), Handle = "@pivoter", Name = "Pivoter" };
        var posts = new List<ContentPost>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BuilderProfileId = builder.Id,
                Content = "After 3 months of zero traction, decided to pivot away from B2C to developer tooling.",
                PublishedAt = new DateTime(2023, 7, 1),
                Likes = 85,
                Replies = 22
            },
            new()
            {
                Id = Guid.NewGuid(),
                BuilderProfileId = builder.Id,
                Content = "Post-mortem and lesson learned: marketing to consumers with zero budget is an uphill battle.",
                PublishedAt = new DateTime(2023, 7, 10),
                Likes = 120,
                Replies = 30
            }
        };

        // Act
        var events = _engine.ReconstructJourney(builder, posts);

        // Assert
        events.Should().HaveCount(2);
        events.Should().Contain(e => e.Category == JourneyEventCategory.Pivot);
        events.Should().Contain(e => e.Category == JourneyEventCategory.FailureOrLesson);
    }

    [Fact]
    public void GetReconstructionLimitations_ShouldIncludeMandatoryPlatformDisclosures()
    {
        // Arrange
        var builder = new BuilderProfile { Id = Guid.NewGuid(), Handle = "@test", Name = "Test" };

        // Act
        var limitations = _engine.GetReconstructionLimitations(builder, 10, 2);

        // Assert
        limitations.Should().NotBeEmpty();
        limitations.Should().Contain(l => l.Contains("strictly limited to publicly available, authorized social posts"));
        limitations.Should().Contain(l => l.Contains("Private metrics"));
        limitations.Should().Contain(l => l.Contains("fewer than 20 posts analyzed"));
    }
}

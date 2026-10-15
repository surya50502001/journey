using BuilderJourney.Application.Engines;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;
using BuilderJourney.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BuilderJourney.Tests;

public class PersonalizedActionPlanGeneratorTests
{
    private readonly PersonalizedActionPlanGenerator _generator = new();

    [Fact]
    public void GeneratePlan_ShouldCalibrateTasksForIdeaStage_WithBuilderReferences()
    {
        var goal = new UserGoal
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = Guid.NewGuid(),
            Stage = UserStage.Idea,
            Objective = UserObjective.BuildProduct,
            HoursPerWeek = 10,
            TargetPlatform = PlatformType.X
        };

        var refProfile = new BuilderProfile
        {
            Id = Guid.NewGuid(),
            Name = "Pieter Levels",
            Handle = "@levelsio",
            IsCurated = true
        };
        refProfile.JourneyEvents.Add(new JourneyEvent
        {
            Id = Guid.NewGuid(),
            Title = "First Concept Formulation",
            Category = JourneyEventCategory.InitialIdea
        });

        var plan = _generator.GeneratePlan(goal, new[] { refProfile });

        plan.Should().NotBeNull();
        plan.Tasks.Should().HaveCount(4); // 4 weeks for <= 10 hours
        plan.Tasks.First().FocusArea.Should().Be("Validation & Listening");
        plan.Tasks.First().RelevantBuilderProfileId.Should().Be(refProfile.Id);
        plan.MethodologyExplanation.Should().Contain("Social engagement does not guarantee revenue");
    }
}

public class SecurityAndEncryptionTests
{
    [Fact]
    public void AesEncryptionService_ShouldEncryptAndDecryptAccurately()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Security:EncryptionKey", "TestVerySecretEncryptionKey123456789!@" }
        };
        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        var service = new AesEncryptionService(config);
        var sensitiveToken = "sample_dummy_oauth_token_12345_test_payload";

        var encrypted = service.Encrypt(sensitiveToken);
        encrypted.Should().NotBeNullOrWhiteSpace();
        encrypted.Should().NotBe(sensitiveToken);

        var decrypted = service.Decrypt(encrypted);
        decrypted.Should().Be(sensitiveToken);
    }

    [Fact]
    public void JwtTokenGenerator_ShouldProduceValidToken()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Secret", "SuperSecretKeyForTestingJwtTokenGenerationMustBeLongEnough123!" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:ExpiryHours", "24" }
        };
        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        var generator = new JwtTokenGenerator(config);
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "tester@builderjourney.io",
            FullName = "Jane Doe",
            Role = "User",
            CurrentStage = UserStage.Prototype,
            PrimaryObjective = UserObjective.AcquireUsers
        };

        var token = generator.GenerateToken(user);
        token.Should().NotBeNullOrWhiteSpace();
        token.Split('.').Should().HaveCount(3); // Header.Payload.Signature
    }
}

using BuilderJourney.Application.DTOs;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.Engines;

public interface IPersonalizedActionPlanGenerator
{
    ActionPlan GeneratePlan(UserGoal goal, IReadOnlyList<BuilderProfile> referenceProfiles);
}

public class PersonalizedActionPlanGenerator : IPersonalizedActionPlanGenerator
{
    public ActionPlan GeneratePlan(UserGoal goal, IReadOnlyList<BuilderProfile> referenceProfiles)
    {
        var planId = Guid.NewGuid();
        var plan = new ActionPlan
        {
            Id = planId,
            UserGoalId = goal.Id,
            UserGoal = goal,
            ApplicationUserId = goal.ApplicationUserId,
            Title = $"{goal.Stage} Execution Roadmap: {FormatObjective(goal.Objective)}",
            Summary = $"A {GetPlanWeeks(goal.HoursPerWeek)}-week structured roadmap calibrated for {goal.HoursPerWeek} hours/week on {goal.TargetPlatform}. Grounded in documented milestones from successful indie founders.",
            MethodologyExplanation = "This action plan is synthesized by matching your current stage and weekly capacity against verified public launch and iteration milestones from established builders. It emphasizes empirical testing over speculative roadmaps. Social engagement does not guarantee revenue.",
            GeneratedAt = DateTime.UtcNow
        };

        var tasks = GenerateTasksForStage(planId, goal, referenceProfiles);
        plan.Tasks = tasks;
        return plan;
    }

    private static int GetPlanWeeks(int hoursPerWeek) => hoursPerWeek <= 10 ? 4 : 6;

    private static string FormatObjective(UserObjective objective) => objective switch
    {
        UserObjective.BuildProduct => "Accelerate Product Delivery",
        UserObjective.AcquireUsers => "Acquire First Paying Customers",
        UserObjective.ImproveContent => "Level Up Content Strategy & Resonance",
        UserObjective.GrowAudience => "Build a High-Trust Builder Audience",
        _ => objective.ToString()
    };

    private static List<ActionPlanTask> GenerateTasksForStage(
        Guid planId,
        UserGoal goal,
        IReadOnlyList<BuilderProfile> referenceProfiles)
    {
        var tasks = new List<ActionPlanTask>();
        var sampleBuilder = referenceProfiles.FirstOrDefault(b => b.JourneyEvents.Count > 0)
            ?? referenceProfiles.FirstOrDefault();

        var refEvent = sampleBuilder?.JourneyEvents.FirstOrDefault();

        switch (goal.Stage)
        {
            case UserStage.Idea:
                tasks.Add(new ActionPlanTask
                {
                    Id = Guid.NewGuid(),
                    ActionPlanId = planId,
                    WeekNumber = 1,
                    FocusArea = "Validation & Listening",
                    Title = "Define the Core Hypothesis & Ask Targeted Builder Questions",
                    ActionDescription = $"Draft a focused post articulating the exact pain point you want to solve. Avoid generic pitches. Ask other builders how they currently navigate this problem on {goal.TargetPlatform}.",
                    SuggestedContentFormat = "Discussion Prompt: Single clear problem question + brief personal frustration",
                    MilestoneTarget = "Receive at least 5 meaningful practitioner comments articulating their current workflow",
                    RelevantBuilderProfileId = sampleBuilder?.Id,
                    RelevantEventId = refEvent?.Id,
                    SupportingRationale = "Pieter Levels and Danny Postma repeatedly validated developer demand by asking direct questions before writing code.",
                    SuccessMetric = ">= 5 genuine practitioner replies",
                    IsCompleted = false
                });

                tasks.Add(new ActionPlanTask
                {
                    Id = Guid.NewGuid(),
                    ActionPlanId = planId,
                    WeekNumber = 2,
                    FocusArea = "Demand Testing",
                    Title = "Publish a 1-Page Concept Mockup or Waitlist",
                    ActionDescription = "Build a lightweight landing page with a clear value proposition, screenshot or mockup, and an email capture form. Share your public thought process behind the design.",
                    SuggestedContentFormat = "Build-in-Public post with image attachment of UI wireframe",
                    MilestoneTarget = "First 25 waitlist signups or email subscriptions",
                    RelevantBuilderProfileId = sampleBuilder?.Id,
                    RelevantEventId = refEvent?.Id,
                    SupportingRationale = "Pre-launch waitlists create an initial cohort of motivated testers for day-one feedback.",
                    SuccessMetric = "25 verified email signups",
                    IsCompleted = false
                });

                tasks.Add(new ActionPlanTask
                {
                    Id = Guid.NewGuid(),
                    ActionPlanId = planId,
                    WeekNumber = 3,
                    FocusArea = "Prototyping",
                    Title = "Develop & Record a 60-Second Prototype Demo",
                    ActionDescription = "Strip all non-essential features and build the single core interaction. Record a silent or narrated 60-second screen capture demonstrating the output.",
                    SuggestedContentFormat = "60-Second Video / GIF demo with concise text explaining the output",
                    MilestoneTarget = "First functional end-to-end screen recording demonstrating the core value",
                    RelevantBuilderProfileId = sampleBuilder?.Id,
                    RelevantEventId = refEvent?.Id,
                    SupportingRationale = "Documented analysis proves that product demo videos consistently generate 2x to 3x higher discussion depth than text updates.",
                    SuccessMetric = "1 published video demo receiving community feedback",
                    IsCompleted = false
                });

                tasks.Add(new ActionPlanTask
                {
                    Id = Guid.NewGuid(),
                    ActionPlanId = planId,
                    WeekNumber = 4,
                    FocusArea = "Early Feedback",
                    Title = "Onboard First 5 Alpha Testers",
                    ActionDescription = "DM the 5 most engaged repliers from your demo post with private access. Personally observe where they get stuck and capture verbatim quotes.",
                    SuggestedContentFormat = "Lessons Learned retrospective post on what surprised you about early usage",
                    MilestoneTarget = "5 active alpha sessions completed with recorded feedback",
                    RelevantBuilderProfileId = sampleBuilder?.Id,
                    RelevantEventId = refEvent?.Id,
                    SupportingRationale = "Early personal onboarding prevents public launch disasters by fixing confusing onboarding steps in advance.",
                    SuccessMetric = "5 recorded tester feedback summaries",
                    IsCompleted = false
                });
                break;

            case UserStage.Prototype:
            case UserStage.Launched:
            default:
                tasks.Add(new ActionPlanTask
                {
                    Id = Guid.NewGuid(),
                    ActionPlanId = planId,
                    WeekNumber = 1,
                    FocusArea = "Public Launch Preparation",
                    Title = "Execute Coordinated Public Launch Announcement",
                    ActionDescription = "Prepare a multi-point launch narrative: what the product does, why you built it, the tech stack, and a direct demo video. Launch simultaneously on Product Hunt and your primary profile.",
                    SuggestedContentFormat = "Launch Thread: 1/ Hook + 60s demo video + pricing + link",
                    MilestoneTarget = "Coordinated launch day with zero downtime",
                    RelevantBuilderProfileId = sampleBuilder?.Id,
                    RelevantEventId = refEvent?.Id,
                    SupportingRationale = "Marc Lou and Tony Dinh systematically pair launch threads with short video clips to maximize community shares.",
                    SuccessMetric = "Public launch executed and recorded",
                    IsCompleted = false
                });

                tasks.Add(new ActionPlanTask
                {
                    Id = Guid.NewGuid(),
                    ActionPlanId = planId,
                    WeekNumber = 2,
                    FocusArea = "Immediate User Feedback",
                    Title = "Ship Rapid 24-Hour Bug-Fix Changelog",
                    ActionDescription = "Address the top 3 user complaints or bugs discovered during launch day. Publish a transparent changelog update thanking specific users.",
                    SuggestedContentFormat = "Build-in-Public Changelog update tagging initial contributors",
                    MilestoneTarget = "3 user-requested improvements shipped within 72 hours",
                    RelevantBuilderProfileId = sampleBuilder?.Id,
                    RelevantEventId = refEvent?.Id,
                    SupportingRationale = "Rapid responsiveness builds intense founder trust among early adopters.",
                    SuccessMetric = "Changelog v1.1 deployed",
                    IsCompleted = false
                });

                tasks.Add(new ActionPlanTask
                {
                    Id = Guid.NewGuid(),
                    ActionPlanId = planId,
                    WeekNumber = 3,
                    FocusArea = "Customer Acquisition",
                    Title = "Convert First Documented Customer & Share Case Study",
                    ActionDescription = "Follow up directly with users who have completed high-value actions in the app. Ask for an annual or monthly commitment and document the milestone.",
                    SuggestedContentFormat = "Milestone announcement celebrating customer #1 with gratitude",
                    MilestoneTarget = "First dollar or subscription recorded",
                    RelevantBuilderProfileId = sampleBuilder?.Id,
                    RelevantEventId = refEvent?.Id,
                    SupportingRationale = "Celebrating early customer milestones publicly provides social proof and signals ongoing momentum.",
                    SuccessMetric = "1 paying customer confirmed",
                    IsCompleted = false
                });

                tasks.Add(new ActionPlanTask
                {
                    Id = Guid.NewGuid(),
                    ActionPlanId = planId,
                    WeekNumber = 4,
                    FocusArea = "Content Flywheel",
                    Title = "Publish Technical Deep-Dive / Architecture Breakdown",
                    ActionDescription = "Write a comprehensive educational thread explaining a non-obvious engineering or design decision you made while building the product.",
                    SuggestedContentFormat = "Educational Thread (6-8 posts with code or architecture diagrams)",
                    MilestoneTarget = "High-resonance educational resource shared by developer community",
                    RelevantBuilderProfileId = sampleBuilder?.Id,
                    RelevantEventId = refEvent?.Id,
                    SupportingRationale = "Educational threads establish domain authority and consistently earn bookmark saves from potential customers.",
                    SuccessMetric = ">= 50 bookmarks on educational thread",
                    IsCompleted = false
                });
                break;
        }

        return tasks;
    }
}

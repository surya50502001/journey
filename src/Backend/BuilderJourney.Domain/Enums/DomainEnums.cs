namespace BuilderJourney.Domain.Enums;

public enum PlatformType
{
    Curated = 0,
    X = 1,
    YouTube = 2,
    Instagram = 3,
    TikTok = 4,
    LinkedIn = 5
}

public enum JourneyEventCategory
{
    InitialIdea = 1,
    ProblemValidation = 2,
    PrototypeDemo = 3,
    PublicLaunch = 4,
    FirstCustomer = 5,
    ProductIteration = 6,
    Pivot = 7,
    AudienceMilestone = 8,
    BusinessMilestone = 9,
    FailureOrLesson = 10,
    Growth = 11
}

public enum ConfidenceLevel
{
    Low = 1,
    Medium = 2,
    High = 3
}

public enum DatePrecision
{
    Exact = 1,
    Month = 2,
    Year = 3,
    Approximate = 4
}

public enum EvidenceType
{
    DirectStatement = 1,
    MetricSurge = 2,
    TemporalContext = 3,
    MediaDemo = 4,
    PublicLink = 5
}

public enum ContentCategory
{
    BuildInPublic = 1,
    ProductDemo = 2,
    TechnicalTutorial = 3,
    EducationalThread = 4,
    PersonalStory = 5,
    LaunchAnnouncement = 6,
    CustomerFeedback = 7,
    LessonsLearned = 8,
    DiscussionQuestion = 9,
    Promotional = 10
}

public enum UserStage
{
    Idea = 1,
    Prototype = 2,
    Launched = 3,
    FirstUsers = 4,
    Growth = 5
}

public enum UserObjective
{
    BuildProduct = 1,
    AcquireUsers = 2,
    ImproveContent = 3,
    GrowAudience = 4
}

public enum VisibilityType
{
    Private = 0,
    Unlisted = 1,
    Public = 2
}

public enum SyncStatus
{
    Queued = 1,
    Running = 2,
    Completed = 3,
    Failed = 4,
    RateLimited = 5
}

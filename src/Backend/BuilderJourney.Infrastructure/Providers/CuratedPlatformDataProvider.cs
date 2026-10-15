using BuilderJourney.Domain.Enums;
using BuilderJourney.Domain.Interfaces;

namespace BuilderJourney.Infrastructure.Providers;

public class CuratedPlatformDataProvider : IPlatformDataProvider
{
    public PlatformType Platform => PlatformType.Curated;

    public PlatformCapability GetCapabilities()
    {
        return new PlatformCapability
        {
            Platform = PlatformType.Curated,
            SupportsLikes = true,
            SupportsReplies = true,
            SupportsReposts = true,
            SupportsViews = true,
            SupportsBookmarks = true,
            SupportsHistoricalFollowers = true,
            SupportsFullHistory = true,
            RateLimitDescription = "No rate limits for verified curated public profiles.",
            DataRetentionPolicy = "Curated public dataset archived for builder educational research.",
            AttributionRequirement = "Attributed to public author handles.",
            Limitations = new List<string>
            {
                "Contains curated public milestone posts; private Stripe transactions and unpublished iterations are not accessible."
            }
        };
    }

    public Task<PlatformUserProfile?> GetUserProfileAsync(string handleOrId, CancellationToken ct = default)
    {
        var clean = handleOrId.Trim().TrimStart('@').ToLowerInvariant();
        return Task.FromResult(CuratedBuilders.FirstOrDefault(b => b.Username.ToLower() == clean));
    }

    public Task<IReadOnlyList<PlatformPostData>> GetUserPostsAsync(string providerUserId, DateTime? since = null, int maxResults = 100, CancellationToken ct = default)
    {
        if (CuratedPosts.TryGetValue(providerUserId, out var posts))
        {
            var filtered = posts.AsEnumerable();
            if (since.HasValue)
            {
                filtered = filtered.Where(p => p.PublishedAt >= since.Value);
            }
            return Task.FromResult<IReadOnlyList<PlatformPostData>>(filtered.Take(maxResults).ToList());
        }

        return Task.FromResult<IReadOnlyList<PlatformPostData>>(Array.Empty<PlatformPostData>());
    }

    public Task<PlatformAudienceMetrics?> GetAudienceMetricsAsync(string providerUserId, CancellationToken ct = default)
    {
        var profile = CuratedBuilders.FirstOrDefault(b => b.ProviderUserId == providerUserId);
        if (profile != null)
        {
            return Task.FromResult<PlatformAudienceMetrics?>(new PlatformAudienceMetrics(
                profile.FollowersCount,
                profile.FollowingCount,
                1500,
                profile.TotalPostsCount,
                DateTime.UtcNow
            ));
        }

        return Task.FromResult<PlatformAudienceMetrics?>(null);
    }

    public static readonly List<PlatformUserProfile> CuratedBuilders = new()
    {
        new(
            "levelsio_id",
            "levelsio",
            "Pieter Levels",
            "Bootstrapping @NomadList, @RemoteOK, and @PhotoAI. 12 startups in 12 months challenger.",
            "https://pbs.twimg.com/profile_images/1798363365851860992/oPzG2GzS_400x400.jpg",
            "https://x.com/levelsio",
            542000,
            1200,
            18400,
            new DateTime(2014, 3, 1, 0, 0, 0, DateTimeKind.Utc)
        ),
        new(
            "marclou_id",
            "marc_louvion",
            "Marc Lou",
            "Solopreneur shipping products with Next.js boilerplate @ShipFast, @Habity, and @ByeByeBye.",
            "https://pbs.twimg.com/profile_images/1690467576266395648/qL8w9D5m_400x400.jpg",
            "https://x.com/marc_louvion",
            148000,
            420,
            6200,
            new DateTime(2021, 6, 1, 0, 0, 0, DateTimeKind.Utc)
        ),
        new(
            "dannypostma_id",
            "dannypostmaa",
            "Danny Postma",
            "Building @HeadshotPro & @Landingfolio. AI founder bootstrapped to $3M+ ARR.",
            "https://pbs.twimg.com/profile_images/1758832049817169920/144JvH5J_400x400.jpg",
            "https://x.com/dannypostmaa",
            172000,
            680,
            8900,
            new DateTime(2018, 1, 15, 0, 0, 0, DateTimeKind.Utc)
        ),
        new(
            "tonydinh_id",
            "tdinh_l",
            "Tony Dinh",
            "Software engineer turned indie builder. Creator of @TypingMind, @DevUtils_app, and @BlackMagic_so.",
            "https://pbs.twimg.com/profile_images/1647466184518410240/J0tXpT0__400x400.jpg",
            "https://x.com/tdinh_l",
            121000,
            390,
            7400,
            new DateTime(2021, 9, 1, 0, 0, 0, DateTimeKind.Utc)
        )
    };

    public static readonly Dictionary<string, List<PlatformPostData>> CuratedPosts = new()
    {
        ["levelsio_id"] = new()
        {
            new(
                "p_levels_1",
                "I'm starting a new challenge: 12 startups in 12 months. Day 1 of building project 1. No funding, just pure HTML, PHP, and jQuery.",
                new DateTime(2014, 3, 10, 14, 30, 0, DateTimeKind.Utc),
                "https://x.com/levelsio/status/p_levels_1",
                "Tweet",
                new() { "https://images.unsplash.com/photo-1519389950473-47ba0277781c" },
                1840,
                312,
                420,
                142000,
                820
            ),
            new(
                "p_levels_2",
                "Quick demo of Nomad List v0.1: an index of cities ranked by cost of living, internet speed, and weather. Would you use this?",
                new DateTime(2014, 7, 24, 16, 0, 0, DateTimeKind.Utc),
                "https://x.com/levelsio/status/p_levels_2",
                "Tweet",
                new() { "https://images.unsplash.com/photo-1486312338219-ce68d2c6f44d" },
                3420,
                640,
                580,
                310000,
                1420
            ),
            new(
                "p_levels_3",
                "Just officially launched Nomad List on Product Hunt! We're live today. Check out the leaderboard and let me know your city rankings.",
                new DateTime(2014, 7, 29, 9, 15, 0, DateTimeKind.Utc),
                "https://x.com/levelsio/status/p_levels_3",
                "Tweet",
                new(),
                4890,
                920,
                880,
                450000,
                1950
            ),
            new(
                "p_levels_4",
                "Got my first paying customer for the Nomad List community! $5 for Slack membership. A real human entered their credit card. Ramen profitable!",
                new DateTime(2014, 8, 12, 18, 45, 0, DateTimeKind.Utc),
                "https://x.com/levelsio/status/p_levels_4",
                "Tweet",
                new(),
                6200,
                1120,
                740,
                520000,
                2450
            ),
            new(
                "p_levels_5",
                "What went wrong with Startup #4: I spent 3 weeks building an automated booking tool without talking to hotels. Lesson learned: do not automate before you validate manual demand.",
                new DateTime(2015, 1, 15, 11, 0, 0, DateTimeKind.Utc),
                "https://x.com/levelsio/status/p_levels_5",
                "Tweet",
                new(),
                4120,
                490,
                610,
                380000,
                3100
            ),
            new(
                "p_levels_6",
                "Shipped v2 of Remote OK: redesigned the job board layout and added automated tags for developers. Traffic surge +140% this week.",
                new DateTime(2016, 4, 18, 13, 20, 0, DateTimeKind.Utc),
                "https://x.com/levelsio/status/p_levels_6",
                "Tweet",
                new() { "https://images.unsplash.com/photo-1460925895917-afdab827c52f" },
                3980,
                380,
                410,
                290000,
                980
            ),
            new(
                "p_levels_7",
                "Just launched Photo AI on Product Hunt! Take synthetic photorealistic portraits with Stable Diffusion. First 100 paying customers signed up in 3 hours.",
                new DateTime(2022, 10, 28, 8, 30, 0, DateTimeKind.Utc),
                "https://x.com/levelsio/status/p_levels_7",
                "Tweet",
                new() { "https://images.unsplash.com/photo-1534528741775-53994a69daeb" },
                12400,
                1890,
                2100,
                1250000,
                4800
            ),
            new(
                "p_levels_8",
                "Reached 500k followers on X! Grateful for everyone following this 10-year journey from a laptop in Chiang Mai to running solo SaaS.",
                new DateTime(2024, 1, 10, 15, 0, 0, DateTimeKind.Utc),
                "https://x.com/levelsio/status/p_levels_8",
                "Tweet",
                new(),
                15600,
                2400,
                1800,
                1600000,
                3900
            )
        },
        ["marclou_id"] = new()
        {
            new(
                "p_marc_1",
                "Day 1 of building ShipFast: a Next.js boilerplate with Stripe, Supabase, and Mailgun already configured so founders can launch in 24 hours.",
                new DateTime(2023, 7, 1, 10, 0, 0, DateTimeKind.Utc),
                "https://x.com/marc_louvion/status/p_marc_1",
                "Tweet",
                new(),
                1420,
                280,
                190,
                120000,
                640
            ),
            new(
                "p_marc_2",
                "Recorded a 60-second video demo of how fast you can set up Stripe payments with ShipFast. Watch how the webhook triggers automatically.",
                new DateTime(2023, 7, 12, 14, 0, 0, DateTimeKind.Utc),
                "https://x.com/marc_louvion/status/p_marc_2",
                "Tweet",
                new() { "https://images.unsplash.com/photo-1551288049-bebda4e38f71" },
                3890,
                620,
                480,
                320000,
                1890
            ),
            new(
                "p_marc_3",
                "Officially launched ShipFast today! Now live for everyone. Stop reinventing user auth and checkout for every single micro-SaaS.",
                new DateTime(2023, 7, 18, 9, 0, 0, DateTimeKind.Utc),
                "https://x.com/marc_louvion/status/p_marc_3",
                "Tweet",
                new(),
                5600,
                940,
                820,
                480000,
                2400
            ),
            new(
                "p_marc_4",
                "First paying customer bought ShipFast 8 minutes after launch! $169 in the bank. Validation complete.",
                new DateTime(2023, 7, 18, 12, 30, 0, DateTimeKind.Utc),
                "https://x.com/marc_louvion/status/p_marc_4",
                "Tweet",
                new(),
                4100,
                580,
                390,
                290000,
                1100
            ),
            new(
                "p_marc_5",
                "Hit $50k MRR milestone across my micro-SaaS portfolio! 100% solo founder, 0 employees, automated customer support.",
                new DateTime(2024, 2, 5, 17, 0, 0, DateTimeKind.Utc),
                "https://x.com/marc_louvion/status/p_marc_5",
                "Tweet",
                new(),
                9200,
                1450,
                1120,
                890000,
                3800
            )
        },
        ["tonydinh_id"] = new()
        {
            new(
                "p_tony_1",
                "Today I quit my software engineering job to build my own Mac apps full-time. Day 1 of building DevUtils.",
                new DateTime(2021, 9, 15, 11, 0, 0, DateTimeKind.Utc),
                "https://x.com/tdinh_l/status/p_tony_1",
                "Tweet",
                new(),
                3800,
                510,
                420,
                310000,
                1200
            ),
            new(
                "p_tony_2",
                "Built an early prototype UI for ChatGPT on the desktop: TypingMind. You just enter your OpenAI API key and get a snappy web client.",
                new DateTime(2023, 3, 6, 16, 20, 0, DateTimeKind.Utc),
                "https://x.com/tdinh_l/status/p_tony_2",
                "Tweet",
                new() { "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe" },
                7800,
                1200,
                1450,
                720000,
                4100
            ),
            new(
                "p_tony_3",
                "Officially launched TypingMind! In less than 24 hours, over 1,000 active users and reached #1 Product of the Day on Product Hunt.",
                new DateTime(2023, 3, 9, 8, 0, 0, DateTimeKind.Utc),
                "https://x.com/tdinh_l/status/p_tony_3",
                "Tweet",
                new(),
                6400,
                980,
                890,
                590000,
                2900
            ),
            new(
                "p_tony_4",
                "TypingMind just reached $100k ARR in 3 months! Documenting the tech stack: Next.js static export hosted on Vercel with zero backend server costs.",
                new DateTime(2023, 6, 12, 13, 0, 0, DateTimeKind.Utc),
                "https://x.com/tdinh_l/status/p_tony_4",
                "Tweet",
                new(),
                8900,
                1340,
                1280,
                850000,
                5200
            )
        },
        ["dannypostma_id"] = new()
        {
            new(
                "p_danny_1",
                "Started experimenting with Stable Diffusion for generating corporate headshots. What if you could get LinkedIn photos without a photographer?",
                new DateTime(2022, 11, 10, 15, 0, 0, DateTimeKind.Utc),
                "https://x.com/dannypostmaa/status/p_danny_1",
                "Tweet",
                new(),
                2900,
                410,
                320,
                240000,
                1100
            ),
            new(
                "p_danny_2",
                "Launched HeadshotPro today! Upload 15 selfies and receive 120 professional studio-quality headshots in 2 hours.",
                new DateTime(2023, 1, 15, 9, 0, 0, DateTimeKind.Utc),
                "https://x.com/dannypostmaa/status/p_danny_2",
                "Tweet",
                new() { "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d" },
                6200,
                980,
                820,
                640000,
                3200
            ),
            new(
                "p_danny_3",
                "Post-mortem on my first failed AI product: why photobooth apps with low retention fail unless you solve an ongoing business workflow.",
                new DateTime(2023, 5, 20, 14, 0, 0, DateTimeKind.Utc),
                "https://x.com/dannypostmaa/status/p_danny_3",
                "Tweet",
                new(),
                4900,
                720,
                680,
                480000,
                3900
            )
        }
    };
}

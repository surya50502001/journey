using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Engines;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.Services;

public class ReportService : IReportService
{
    private readonly IAppDbContext _db;
    private readonly IJourneyService _journeyService;
    private readonly IContentAnalysisService _contentService;
    private readonly IBreakthroughService _breakthroughService;

    public ReportService(
        IAppDbContext db,
        IJourneyService journeyService,
        IContentAnalysisService contentService,
        IBreakthroughService breakthroughService)
    {
        _db = db;
        _journeyService = journeyService;
        _contentService = contentService;
        _breakthroughService = breakthroughService;
    }

    public async Task<ApiResponse<JourneyReportDto>> GetOrCreateReportAsync(Guid builderId, CancellationToken ct = default)
    {
        var builder = await _db.BuilderProfiles
            .Include(b => b.BuilderCategory)
            .Include(b => b.Reports)
                .ThenInclude(r => r.SharingSetting)
            .FirstOrDefaultAsync(b => b.Id == builderId, ct);

        if (builder == null)
        {
            return ApiResponse<JourneyReportDto>.Fail("Builder profile not found.");
        }

        var timelineRes = await _journeyService.GetTimelineAsync(builderId, ct);
        var contentRes = await _contentService.GetContentStrategyAsync(builderId, ct);
        var breakthroughRes = await _breakthroughService.GetBreakthroughsAsync(builderId, ct);

        var timeline = timelineRes.Data ?? new JourneyTimelineDto(builder.Id, builder.Name, builder.Handle, builder.AvatarUrl, null, "", 0, 0, 0, new(), new());
        var content = contentRes.Data ?? new ContentStrategySummaryDto(builder.Id, builder.Name, builder.Handle, 0, null, null, 0, "", new(), new(), new(), new(), new(), "", "");
        var breakthroughs = breakthroughRes.Data ?? new List<BreakthroughDto>();

        var existingReport = builder.Reports.OrderByDescending(r => r.GeneratedAt).FirstOrDefault();
        if (existingReport == null)
        {
            var lessons = GenerateLessonsFromJourney(builder, timeline);
            existingReport = new JourneyReport
            {
                Id = Guid.NewGuid(),
                BuilderProfileId = builder.Id,
                BuilderProfile = builder,
                Title = $"{builder.Name}'s Builder Journey Intelligence Report",
                ExecutiveSummary = GenerateExecutiveSummary(builder, timeline, content),
                ContentDnaSummary = content.StrategyOverview,
                KeyLessonsJson = JsonSerializer.Serialize(lessons),
                LimitationsDisclosed = string.Join("; ", timeline.ReconstructionLimitations),
                GeneratedAt = DateTime.UtcNow,
                Version = "1.0",
                SharingSetting = new ReportSharingSetting
                {
                    Id = Guid.NewGuid(),
                    ShareSlug = Guid.NewGuid().ToString("N")[..12],
                    Visibility = VisibilityType.Public,
                    AllowCloning = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _db.JourneyReports.Add(existingReport);
            await _db.SaveChangesAsync(ct);
        }

        var lessonsList = ParseLessons(existingReport.KeyLessonsJson);
        var reportDto = new JourneyReportDto(
            existingReport.Id,
            builder.Id,
            builder.Name,
            builder.Handle,
            builder.AvatarUrl,
            builder.Bio,
            builder.BuilderCategory?.Name ?? "General",
            builder.CurrentStatus,
            builder.DocumentedStartDate,
            existingReport.Title,
            existingReport.ExecutiveSummary,
            existingReport.ContentDnaSummary,
            lessonsList,
            existingReport.LimitationsDisclosed,
            existingReport.GeneratedAt,
            existingReport.Version,
            existingReport.SharingSetting?.ShareSlug,
            existingReport.SharingSetting?.Visibility ?? VisibilityType.Public,
            timeline,
            content,
            breakthroughs
        );

        return ApiResponse<JourneyReportDto>.Ok(reportDto);
    }

    public async Task<ApiResponse<JourneyReportDto>> GetReportByShareSlugAsync(string shareSlug, CancellationToken ct = default)
    {
        var cleanSlug = shareSlug.Trim();
        var report = await _db.JourneyReports
            .Include(r => r.SharingSetting)
            .Include(r => r.BuilderProfile)
                .ThenInclude(b => b.BuilderCategory)
            .FirstOrDefaultAsync(r => r.SharingSetting != null && r.SharingSetting.ShareSlug == cleanSlug, ct);

        if (report == null || report.SharingSetting?.Visibility == VisibilityType.Private)
        {
            return ApiResponse<JourneyReportDto>.Fail("Report not found or not publicly accessible.");
        }

        return await GetOrCreateReportAsync(report.BuilderProfileId, ct);
    }

    public async Task<ApiResponse<JourneyReportDto>> UpdateSharingSettingsAsync(Guid reportId, UpdateSharingSettingsRequest request, CancellationToken ct = default)
    {
        var report = await _db.JourneyReports
            .Include(r => r.SharingSetting)
            .FirstOrDefaultAsync(r => r.Id == reportId, ct);

        if (report == null)
        {
            return ApiResponse<JourneyReportDto>.Fail("Report not found.");
        }

        if (report.SharingSetting == null)
        {
            report.SharingSetting = new ReportSharingSetting
            {
                Id = Guid.NewGuid(),
                JourneyReportId = report.Id,
                ShareSlug = Guid.NewGuid().ToString("N")[..12]
            };
        }

        report.SharingSetting.Visibility = request.Visibility;
        report.SharingSetting.AllowCloning = request.AllowCloning;
        await _db.SaveChangesAsync(ct);

        return await GetOrCreateReportAsync(report.BuilderProfileId, ct);
    }

    private static string GenerateExecutiveSummary(BuilderProfile builder, JourneyTimelineDto timeline, ContentStrategySummaryDto content)
    {
        var startYear = builder.DocumentedStartDate?.Year.ToString() ?? "early documentation";
        var milestonesCount = timeline.TotalEvents;
        var topCategory = content.Categories.FirstOrDefault()?.CategoryName ?? "Build-in-Public";

        return $"{builder.Name} ({builder.Handle}) is a public builder active since {startYear}. " +
               $"Their documented journey includes {milestonesCount} verifiable milestones, ranging from initial prototyping to launch cycles. " +
               $"Publishing primarily across {topCategory}, their journey offers a real-world case study in consistent execution and public distribution.";
    }

    private static List<LessonLearnedDto> GenerateLessonsFromJourney(BuilderProfile builder, JourneyTimelineDto timeline)
    {
        var lessons = new List<LessonLearnedDto>();

        var launchEvent = timeline.Events.FirstOrDefault(e => e.Category == JourneyEventCategory.PublicLaunch);
        if (launchEvent != null)
        {
            lessons.Add(new LessonLearnedDto(
                "Launch With High-Fidelity Demonstrations",
                $"{builder.Name} accompanied major launch milestones with direct product screen captures and clear value statements.",
                "Prioritize working software over speculative roadmaps. Present an interactive demo within 60 seconds of user attention.",
                launchEvent.Title
            ));
        }

        var customerEvent = timeline.Events.FirstOrDefault(e => e.Category == JourneyEventCategory.FirstCustomer);
        if (customerEvent != null)
        {
            lessons.Add(new LessonLearnedDto(
                "Validate Monetization Early",
                $"Early customer milestone documented at {customerEvent.EventDate:MMM yyyy}.",
                "Ask for payment early to distinguish genuine operational demand from polite compliments.",
                customerEvent.Title
            ));
        }

        lessons.Add(new LessonLearnedDto(
            "Embrace Iterative Public Shipping",
            $"Across {timeline.TotalEvents} documented milestones, continuous public updates created momentum and community feedback loops.",
            "Post progress micro-updates consistently to build relationship equity before asking for attention.",
            $"Observed across {timeline.TotalEvents} timeline events."
        ));

        return lessons;
    }

    private static List<LessonLearnedDto> ParseLessons(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<LessonLearnedDto>>(json) ?? new List<LessonLearnedDto>();
        }
        catch
        {
            return new List<LessonLearnedDto>();
        }
    }
}

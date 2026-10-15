using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Engines;
using BuilderJourney.Application.Interfaces;

namespace BuilderJourney.Application.Services;

public class JourneyService : IJourneyService
{
    private readonly IAppDbContext _db;
    private readonly IJourneyReconstructionEngine _engine;

    public JourneyService(IAppDbContext db, IJourneyReconstructionEngine engine)
    {
        _db = db;
        _engine = engine;
    }

    public async Task<ApiResponse<JourneyTimelineDto>> GetTimelineAsync(Guid builderId, CancellationToken ct = default)
    {
        var builder = await _db.BuilderProfiles
            .Include(b => b.JourneyEvents)
                .ThenInclude(e => e.Evidences)
                    .ThenInclude(ev => ev.ContentPost)
            .Include(b => b.Posts)
            .FirstOrDefaultAsync(b => b.Id == builderId, ct);

        if (builder == null)
        {
            return ApiResponse<JourneyTimelineDto>.Fail("Builder profile not found.");
        }

        // If no events have been reconstructed yet but posts exist, run reconstruction automatically
        if (builder.JourneyEvents.Count == 0 && builder.Posts.Count > 0)
        {
            return await ReconstructTimelineAsync(builderId, ct);
        }

        var eventDtos = builder.JourneyEvents
            .OrderBy(e => e.EventDate)
            .Select(e => new JourneyEventDto(
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
            ))
            .ToList();

        var highConfCount = eventDtos.Count(e => e.Confidence == Domain.Enums.ConfidenceLevel.High);
        var inferredCount = eventDtos.Count(e => e.IsInferred);
        var limitations = _engine.GetReconstructionLimitations(builder, builder.Posts.Count, eventDtos.Count);

        var timelineDto = new JourneyTimelineDto(
            builder.Id,
            builder.Name,
            builder.Handle,
            builder.AvatarUrl,
            builder.DocumentedStartDate,
            builder.CurrentStatus,
            eventDtos.Count,
            highConfCount,
            inferredCount,
            eventDtos,
            limitations
        );

        return ApiResponse<JourneyTimelineDto>.Ok(timelineDto);
    }

    public async Task<ApiResponse<JourneyTimelineDto>> ReconstructTimelineAsync(Guid builderId, CancellationToken ct = default)
    {
        var builder = await _db.BuilderProfiles
            .Include(b => b.Posts)
            .Include(b => b.JourneyEvents)
                .ThenInclude(e => e.Evidences)
            .FirstOrDefaultAsync(b => b.Id == builderId, ct);

        if (builder == null)
        {
            return ApiResponse<JourneyTimelineDto>.Fail("Builder profile not found.");
        }

        // Clear existing events
        if (builder.JourneyEvents.Count > 0)
        {
            _db.JourneyEvents.RemoveRange(builder.JourneyEvents);
            await _db.SaveChangesAsync(ct);
        }

        var reconstructed = _engine.ReconstructJourney(builder, builder.Posts);
        foreach (var ev in reconstructed)
        {
            _db.JourneyEvents.Add(ev);
        }

        if (reconstructed.Count > 0 && !builder.DocumentedStartDate.HasValue)
        {
            builder.DocumentedStartDate = reconstructed.Min(e => e.EventDate);
        }

        await _db.SaveChangesAsync(ct);

        return await GetTimelineAsync(builderId, ct);
    }
}

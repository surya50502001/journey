using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Engines;
using BuilderJourney.Application.Interfaces;

namespace BuilderJourney.Application.Services;

public class BuilderComparisonService : IBuilderComparisonService
{
    private readonly IAppDbContext _db;
    private readonly IBuilderComparator _comparator;
    private readonly IContentStrategyAnalyzer _contentAnalyzer;

    public BuilderComparisonService(
        IAppDbContext db,
        IBuilderComparator comparator,
        IContentStrategyAnalyzer contentAnalyzer)
    {
        _db = db;
        _comparator = comparator;
        _contentAnalyzer = contentAnalyzer;
    }

    public async Task<ApiResponse<BuilderComparisonResultDto>> CompareBuildersAsync(BuilderComparisonRequest request, CancellationToken ct = default)
    {
        if (request.BuilderIds == null || request.BuilderIds.Count == 0)
        {
            return ApiResponse<BuilderComparisonResultDto>.Fail("At least one builder ID must be specified.");
        }

        var distinctIds = request.BuilderIds.Distinct().Take(4).ToList();
        var builders = await _db.BuilderProfiles
            .Include(b => b.BuilderCategory)
            .Include(b => b.JourneyEvents)
                .ThenInclude(e => e.Evidences)
                    .ThenInclude(ev => ev.ContentPost)
            .Include(b => b.Posts)
            .Where(b => distinctIds.Contains(b.Id))
            .ToListAsync(ct);

        if (builders.Count == 0)
        {
            return ApiResponse<BuilderComparisonResultDto>.Fail("No matching builders found for comparison.");
        }

        var comparisonResult = _comparator.CompareBuilders(builders, _contentAnalyzer);
        return ApiResponse<BuilderComparisonResultDto>.Ok(comparisonResult);
    }
}

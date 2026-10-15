using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Engines;
using BuilderJourney.Application.Interfaces;

namespace BuilderJourney.Application.Services;

public class ContentAnalysisService : IContentAnalysisService
{
    private readonly IAppDbContext _db;
    private readonly IContentStrategyAnalyzer _analyzer;

    public ContentAnalysisService(IAppDbContext db, IContentStrategyAnalyzer analyzer)
    {
        _db = db;
        _analyzer = analyzer;
    }

    public async Task<ApiResponse<ContentStrategySummaryDto>> GetContentStrategyAsync(Guid builderId, CancellationToken ct = default)
    {
        var builder = await _db.BuilderProfiles
            .Include(b => b.Posts)
            .FirstOrDefaultAsync(b => b.Id == builderId, ct);

        if (builder == null)
        {
            return ApiResponse<ContentStrategySummaryDto>.Fail("Builder profile not found.");
        }

        var summary = _analyzer.AnalyzeStrategy(builder, builder.Posts.ToList());
        return ApiResponse<ContentStrategySummaryDto>.Ok(summary);
    }
}

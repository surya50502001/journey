using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Engines;
using BuilderJourney.Application.Interfaces;

namespace BuilderJourney.Application.Services;

public class BreakthroughService : IBreakthroughService
{
    private readonly IAppDbContext _db;
    private readonly IBreakthroughDetector _detector;

    public BreakthroughService(IAppDbContext db, IBreakthroughDetector detector)
    {
        _db = db;
        _detector = detector;
    }

    public async Task<ApiResponse<List<BreakthroughDto>>> GetBreakthroughsAsync(Guid builderId, CancellationToken ct = default)
    {
        var builder = await _db.BuilderProfiles
            .Include(b => b.Posts)
            .FirstOrDefaultAsync(b => b.Id == builderId, ct);

        if (builder == null)
        {
            return ApiResponse<List<BreakthroughDto>>.Fail("Builder profile not found.");
        }

        var breakthroughs = _detector.DetectBreakthroughs(builder, builder.Posts.ToList());
        return ApiResponse<List<BreakthroughDto>>.Ok(breakthroughs);
    }
}

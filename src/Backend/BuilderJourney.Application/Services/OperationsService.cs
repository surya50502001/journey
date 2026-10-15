using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Domain.Enums;
using BuilderJourney.Domain.Interfaces;

namespace BuilderJourney.Application.Services;

public class OperationsService : IOperationsService
{
    private readonly IAppDbContext _db;
    private readonly IEnumerable<IPlatformDataProvider> _providers;

    public OperationsService(IAppDbContext db, IEnumerable<IPlatformDataProvider> providers)
    {
        _db = db;
        _providers = providers;
    }

    public async Task<ApiResponse<List<SyncJobStatusDto>>> GetRecentSyncJobsAsync(CancellationToken ct = default)
    {
        var jobs = await _db.DataSyncJobs
            .Include(j => j.BuilderProfile)
            .OrderByDescending(j => j.StartedAt)
            .Take(20)
            .Select(j => new SyncJobStatusDto(
                j.Id,
                j.BuilderProfileId,
                j.BuilderProfile.Handle,
                j.Status,
                j.ItemsProcessed,
                j.ErrorMessage,
                j.StartedAt,
                j.CompletedAt
            ))
            .ToListAsync(ct);

        return ApiResponse<List<SyncJobStatusDto>>.Ok(jobs);
    }

    public Task<ApiResponse<List<PlatformCapability>>> GetPlatformCapabilitiesAsync(CancellationToken ct = default)
    {
        var capabilities = _providers.Select(p => p.GetCapabilities()).ToList();
        return Task.FromResult(ApiResponse<List<PlatformCapability>>.Ok(capabilities));
    }
}

using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Domain.Entities;

namespace BuilderJourney.Application.Services;

public class WatchlistService : IWatchlistService
{
    private readonly IAppDbContext _db;

    public WatchlistService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<List<WatchlistEntryDto>>> GetWatchlistAsync(Guid userId, CancellationToken ct = default)
    {
        var entries = await _db.UserWatchlists
            .Include(w => w.BuilderProfile)
                .ThenInclude(b => b.BuilderCategory)
            .Include(w => w.BuilderProfile)
                .ThenInclude(b => b.JourneyEvents)
            .Where(w => w.ApplicationUserId == userId)
            .OrderByDescending(w => w.AddedAt)
            .Select(w => new WatchlistEntryDto(
                w.Id,
                w.BuilderProfileId,
                w.BuilderProfile.Name,
                w.BuilderProfile.Handle,
                w.BuilderProfile.AvatarUrl,
                w.BuilderProfile.BuilderCategory.Name,
                w.BuilderProfile.FollowersCount,
                w.BuilderProfile.CurrentStatus,
                w.AddedAt,
                w.Notes,
                w.BuilderProfile.JourneyEvents.OrderByDescending(e => e.EventDate).Select(e => (DateTime?)e.EventDate).FirstOrDefault(),
                w.BuilderProfile.JourneyEvents.OrderByDescending(e => e.EventDate).Select(e => e.Title).FirstOrDefault()
            ))
            .ToListAsync(ct);

        return ApiResponse<List<WatchlistEntryDto>>.Ok(entries);
    }

    public async Task<ApiResponse<bool>> AddToWatchlistAsync(Guid userId, AddToWatchlistRequest request, CancellationToken ct = default)
    {
        var existing = await _db.UserWatchlists
            .FirstOrDefaultAsync(w => w.ApplicationUserId == userId && w.BuilderProfileId == request.BuilderProfileId, ct);

        if (existing != null)
        {
            existing.Notes = request.Notes ?? existing.Notes;
            await _db.SaveChangesAsync(ct);
            return ApiResponse<bool>.Ok(true, "Watchlist entry updated.");
        }

        var entry = new UserWatchlist
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = userId,
            BuilderProfileId = request.BuilderProfileId,
            AddedAt = DateTime.UtcNow,
            Notes = request.Notes?.Trim() ?? string.Empty
        };

        _db.UserWatchlists.Add(entry);
        await _db.SaveChangesAsync(ct);

        return ApiResponse<bool>.Ok(true, "Added to watchlist.");
    }

    public async Task<ApiResponse<bool>> RemoveFromWatchlistAsync(Guid userId, Guid builderId, CancellationToken ct = default)
    {
        var entry = await _db.UserWatchlists
            .FirstOrDefaultAsync(w => w.ApplicationUserId == userId && w.BuilderProfileId == builderId, ct);

        if (entry != null)
        {
            _db.UserWatchlists.Remove(entry);
            await _db.SaveChangesAsync(ct);
        }

        return ApiResponse<bool>.Ok(true, "Removed from watchlist.");
    }
}

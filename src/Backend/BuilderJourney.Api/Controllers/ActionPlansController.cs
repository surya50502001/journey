using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Interfaces;

namespace BuilderJourney.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/action-plans")]
public class ActionPlansController : ControllerBase
{
    private readonly IActionPlanService _actionPlanService;

    public ActionPlansController(IActionPlanService actionPlanService)
    {
        _actionPlanService = actionPlanService;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<ActionPlanDto>>> GeneratePlan(
        [FromBody] GenerateActionPlanRequest request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var response = await _actionPlanService.GenerateActionPlanAsync(userId.Value, request, ct);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    [HttpGet("latest")]
    public async Task<ActionResult<ApiResponse<ActionPlanDto>>> GetLatestPlan(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var response = await _actionPlanService.GetLatestActionPlanAsync(userId.Value, ct);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpPatch("tasks/{taskId:guid}")]
    public async Task<ActionResult<ApiResponse<ActionPlanTaskDto>>> UpdateTaskStatus(
        Guid taskId,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var response = await _actionPlanService.UpdateTaskStatusAsync(userId.Value, taskId, request, ct);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (claim != null && Guid.TryParse(claim.Value, out var id))
        {
            return id;
        }
        return null;
    }
}

[Authorize]
[ApiController]
[Route("api/v1/watchlist")]
public class WatchlistController : ControllerBase
{
    private readonly IWatchlistService _watchlistService;

    public WatchlistController(IWatchlistService watchlistService)
    {
        _watchlistService = watchlistService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<WatchlistEntryDto>>>> GetWatchlist(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var response = await _watchlistService.GetWatchlistAsync(userId.Value, ct);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<bool>>> AddToWatchlist(
        [FromBody] AddToWatchlistRequest request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var response = await _watchlistService.AddToWatchlistAsync(userId.Value, request, ct);
        return Ok(response);
    }

    [HttpDelete("{builderId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveFromWatchlist(
        Guid builderId,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var response = await _watchlistService.RemoveFromWatchlistAsync(userId.Value, builderId, ct);
        return Ok(response);
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (claim != null && Guid.TryParse(claim.Value, out var id))
        {
            return id;
        }
        return null;
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Api.Controllers;

[ApiController]
[Route("api/v1/builders")]
public class BuildersController : ControllerBase
{
    private readonly IBuilderService _builderService;

    public BuildersController(IBuilderService builderService)
    {
        _builderService = builderService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BuilderProfileSummaryDto>>>> GetBuilders(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] PlatformType? platform,
        [FromQuery] bool? curatedOnly,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken ct = default)
    {
        var filter = new BuilderFilterDto(search, category, platform, curatedOnly, sortBy, page, pageSize);
        var response = await _builderService.GetBuildersAsync(filter, ct);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BuilderProfileDetailDto>>> GetBuilderById(Guid id, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var response = await _builderService.GetBuilderByIdAsync(id, userId, ct);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpGet("handle/{handle}")]
    public async Task<ActionResult<ApiResponse<BuilderProfileDetailDto>>> GetBuilderByHandle(string handle, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var response = await _builderService.GetBuilderByHandleAsync(handle, userId, ct);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<List<BuilderCategoryDto>>>> GetCategories(CancellationToken ct)
    {
        var response = await _builderService.GetCategoriesAsync(ct);
        return Ok(response);
    }

    [HttpPost("custom")]
    public async Task<ActionResult<ApiResponse<BuilderProfileDetailDto>>> CreateCustomBuilder(
        [FromBody] CreateCustomBuilderRequest request,
        CancellationToken ct)
    {
        var response = await _builderService.CreateCustomBuilderAsync(request, ct);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    [HttpPost("{id:guid}/sync")]
    public async Task<ActionResult<ApiResponse<SyncJobStatusDto>>> TriggerSync(
        Guid id,
        [FromBody] SyncBuilderRequest request,
        CancellationToken ct)
    {
        var response = await _builderService.TriggerSyncAsync(id, request, ct);
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

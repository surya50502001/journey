using Microsoft.AspNetCore.Mvc;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Interfaces;

namespace BuilderJourney.Api.Controllers;

[ApiController]
[Route("api/v1/content")]
public class ContentAnalysisController : ControllerBase
{
    private readonly IContentAnalysisService _contentService;

    public ContentAnalysisController(IContentAnalysisService contentService)
    {
        _contentService = contentService;
    }

    [HttpGet("{builderId:guid}")]
    public async Task<ActionResult<ApiResponse<ContentStrategySummaryDto>>> GetContentStrategy(Guid builderId, CancellationToken ct)
    {
        var response = await _contentService.GetContentStrategyAsync(builderId, ct);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }
}

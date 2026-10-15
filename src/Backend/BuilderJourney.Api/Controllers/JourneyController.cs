using Microsoft.AspNetCore.Mvc;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Interfaces;

namespace BuilderJourney.Api.Controllers;

[ApiController]
[Route("api/v1/journey")]
public class JourneyController : ControllerBase
{
    private readonly IJourneyService _journeyService;

    public JourneyController(IJourneyService journeyService)
    {
        _journeyService = journeyService;
    }

    [HttpGet("{builderId:guid}")]
    public async Task<ActionResult<ApiResponse<JourneyTimelineDto>>> GetTimeline(Guid builderId, CancellationToken ct)
    {
        var response = await _journeyService.GetTimelineAsync(builderId, ct);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpPost("{builderId:guid}/reconstruct")]
    public async Task<ActionResult<ApiResponse<JourneyTimelineDto>>> ReconstructTimeline(Guid builderId, CancellationToken ct)
    {
        var response = await _journeyService.ReconstructTimelineAsync(builderId, ct);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}

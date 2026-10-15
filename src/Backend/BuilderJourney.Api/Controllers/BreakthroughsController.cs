using Microsoft.AspNetCore.Mvc;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Interfaces;

namespace BuilderJourney.Api.Controllers;

[ApiController]
[Route("api/v1/breakthroughs")]
public class BreakthroughsController : ControllerBase
{
    private readonly IBreakthroughService _breakthroughService;

    public BreakthroughsController(IBreakthroughService breakthroughService)
    {
        _breakthroughService = breakthroughService;
    }

    [HttpGet("{builderId:guid}")]
    public async Task<ActionResult<ApiResponse<List<BreakthroughDto>>>> GetBreakthroughs(Guid builderId, CancellationToken ct)
    {
        var response = await _breakthroughService.GetBreakthroughsAsync(builderId, ct);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }
}

[ApiController]
[Route("api/v1/comparison")]
public class ComparisonController : ControllerBase
{
    private readonly IBuilderComparisonService _comparisonService;

    public ComparisonController(IBuilderComparisonService comparisonService)
    {
        _comparisonService = comparisonService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BuilderComparisonResultDto>>> CompareBuilders(
        [FromBody] BuilderComparisonRequest request,
        CancellationToken ct)
    {
        var response = await _comparisonService.CompareBuildersAsync(request, ct);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}

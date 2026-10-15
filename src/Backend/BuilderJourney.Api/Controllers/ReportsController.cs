using Microsoft.AspNetCore.Mvc;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Domain.Interfaces;

namespace BuilderJourney.Api.Controllers;

[ApiController]
[Route("api/v1/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("builder/{builderId:guid}")]
    public async Task<ActionResult<ApiResponse<JourneyReportDto>>> GetReportByBuilderId(Guid builderId, CancellationToken ct)
    {
        var response = await _reportService.GetOrCreateReportAsync(builderId, ct);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpGet("share/{shareSlug}")]
    public async Task<ActionResult<ApiResponse<JourneyReportDto>>> GetReportByShareSlug(string shareSlug, CancellationToken ct)
    {
        var response = await _reportService.GetReportByShareSlugAsync(shareSlug, ct);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpPut("{reportId:guid}/sharing")]
    public async Task<ActionResult<ApiResponse<JourneyReportDto>>> UpdateSharingSettings(
        Guid reportId,
        [FromBody] UpdateSharingSettingsRequest request,
        CancellationToken ct)
    {
        var response = await _reportService.UpdateSharingSettingsAsync(reportId, request, ct);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}

[ApiController]
[Route("api/v1/operations")]
public class OperationsController : ControllerBase
{
    private readonly IOperationsService _operationsService;

    public OperationsController(IOperationsService operationsService)
    {
        _operationsService = operationsService;
    }

    [HttpGet("sync-jobs")]
    public async Task<ActionResult<ApiResponse<List<SyncJobStatusDto>>>> GetRecentSyncJobs(CancellationToken ct)
    {
        var response = await _operationsService.GetRecentSyncJobsAsync(ct);
        return Ok(response);
    }

    [HttpGet("capabilities")]
    public async Task<ActionResult<ApiResponse<List<PlatformCapability>>>> GetPlatformCapabilities(CancellationToken ct)
    {
        var response = await _operationsService.GetPlatformCapabilitiesAsync(ct);
        return Ok(response);
    }
}

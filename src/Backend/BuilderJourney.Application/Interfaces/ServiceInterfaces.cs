using BuilderJourney.Application.DTOs;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;
using BuilderJourney.Domain.Interfaces;

namespace BuilderJourney.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<ApiResponse<UserProfileDto>> GetCurrentUserProfileAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request, CancellationToken ct = default);
}

public interface IBuilderService
{
    Task<ApiResponse<PagedResult<BuilderProfileSummaryDto>>> GetBuildersAsync(BuilderFilterDto filter, CancellationToken ct = default);
    Task<ApiResponse<BuilderProfileDetailDto>> GetBuilderByIdAsync(Guid id, Guid? currentUserId = null, CancellationToken ct = default);
    Task<ApiResponse<BuilderProfileDetailDto>> GetBuilderByHandleAsync(string handle, Guid? currentUserId = null, CancellationToken ct = default);
    Task<ApiResponse<List<BuilderCategoryDto>>> GetCategoriesAsync(CancellationToken ct = default);
    Task<ApiResponse<BuilderProfileDetailDto>> CreateCustomBuilderAsync(CreateCustomBuilderRequest request, CancellationToken ct = default);
    Task<ApiResponse<SyncJobStatusDto>> TriggerSyncAsync(Guid builderId, SyncBuilderRequest request, CancellationToken ct = default);
}

public interface IJourneyService
{
    Task<ApiResponse<JourneyTimelineDto>> GetTimelineAsync(Guid builderId, CancellationToken ct = default);
    Task<ApiResponse<JourneyTimelineDto>> ReconstructTimelineAsync(Guid builderId, CancellationToken ct = default);
}

public interface IContentAnalysisService
{
    Task<ApiResponse<ContentStrategySummaryDto>> GetContentStrategyAsync(Guid builderId, CancellationToken ct = default);
}

public interface IBreakthroughService
{
    Task<ApiResponse<List<BreakthroughDto>>> GetBreakthroughsAsync(Guid builderId, CancellationToken ct = default);
}

public interface IBuilderComparisonService
{
    Task<ApiResponse<BuilderComparisonResultDto>> CompareBuildersAsync(BuilderComparisonRequest request, CancellationToken ct = default);
}

public interface IActionPlanService
{
    Task<ApiResponse<ActionPlanDto>> GenerateActionPlanAsync(Guid userId, GenerateActionPlanRequest request, CancellationToken ct = default);
    Task<ApiResponse<ActionPlanDto>> GetLatestActionPlanAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse<ActionPlanTaskDto>> UpdateTaskStatusAsync(Guid userId, Guid taskId, UpdateTaskStatusRequest request, CancellationToken ct = default);
}

public interface IWatchlistService
{
    Task<ApiResponse<List<WatchlistEntryDto>>> GetWatchlistAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse<bool>> AddToWatchlistAsync(Guid userId, AddToWatchlistRequest request, CancellationToken ct = default);
    Task<ApiResponse<bool>> RemoveFromWatchlistAsync(Guid userId, Guid builderId, CancellationToken ct = default);
}

public interface IReportService
{
    Task<ApiResponse<JourneyReportDto>> GetOrCreateReportAsync(Guid builderId, CancellationToken ct = default);
    Task<ApiResponse<JourneyReportDto>> GetReportByShareSlugAsync(string shareSlug, CancellationToken ct = default);
    Task<ApiResponse<JourneyReportDto>> UpdateSharingSettingsAsync(Guid reportId, UpdateSharingSettingsRequest request, CancellationToken ct = default);
}

public interface IOperationsService
{
    Task<ApiResponse<List<SyncJobStatusDto>>> GetRecentSyncJobsAsync(CancellationToken ct = default);
    Task<ApiResponse<List<PlatformCapability>>> GetPlatformCapabilitiesAsync(CancellationToken ct = default);
}

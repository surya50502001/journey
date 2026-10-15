using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data, string message = "") =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string error) =>
        new() { Success = false, Errors = new List<string> { error } };

    public static ApiResponse<T> Fail(List<string> errors) =>
        new() { Success = false, Errors = errors };
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / Math.Max(1, PageSize));
}

public record RegisterRequest(
    string Email,
    string Password,
    string FullName,
    UserStage? CurrentStage,
    UserObjective? PrimaryObjective,
    PlatformType? PreferredPlatform,
    int? HoursAvailablePerWeek
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    string Token,
    Guid UserId,
    string Email,
    string FullName,
    string Role,
    UserStage CurrentStage,
    UserObjective PrimaryObjective
);

public record UserProfileDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    UserStage CurrentStage,
    UserObjective PrimaryObjective,
    PlatformType PreferredPlatform,
    int HoursAvailablePerWeek,
    string SkillsAndConstraints,
    DateTime CreatedAt
);

public record UpdateUserProfileRequest(
    string FullName,
    UserStage CurrentStage,
    UserObjective PrimaryObjective,
    PlatformType PreferredPlatform,
    int HoursAvailablePerWeek,
    string SkillsAndConstraints
);

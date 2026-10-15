using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Domain.Entities;
using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _db;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IAppDbContext db, IJwtTokenGenerator jwtTokenGenerator)
    {
        _db = db;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ApiResponse<AuthResponse>.Fail("Email and password are required.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, ct);
        if (existingUser != null)
        {
            return ApiResponse<AuthResponse>.Fail("An account with this email already exists.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            FullName = request.FullName?.Trim() ?? "Builder",
            CurrentStage = request.CurrentStage ?? UserStage.Idea,
            PrimaryObjective = request.PrimaryObjective ?? UserObjective.BuildProduct,
            PreferredPlatform = request.PreferredPlatform ?? PlatformType.X,
            HoursAvailablePerWeek = request.HoursAvailablePerWeek ?? 10,
            CreatedAt = DateTime.UtcNow,
            Role = "User"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        var token = _jwtTokenGenerator.GenerateToken(user);
        var response = new AuthResponse(
            token,
            user.Id,
            user.Email,
            user.FullName,
            user.Role,
            user.CurrentStage,
            user.PrimaryObjective
        );

        return ApiResponse<AuthResponse>.Ok(response, "Account registered successfully.");
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ApiResponse<AuthResponse>.Fail("Email and password are required.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, ct);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ApiResponse<AuthResponse>.Fail("Invalid email or password.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        var token = _jwtTokenGenerator.GenerateToken(user);
        var response = new AuthResponse(
            token,
            user.Id,
            user.Email,
            user.FullName,
            user.Role,
            user.CurrentStage,
            user.PrimaryObjective
        );

        return ApiResponse<AuthResponse>.Ok(response, "Login successful.");
    }

    public async Task<ApiResponse<UserProfileDto>> GetCurrentUserProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _db.Users.FindAsync(new object[] { userId }, ct);
        if (user == null)
        {
            return ApiResponse<UserProfileDto>.Fail("User not found.");
        }

        var dto = new UserProfileDto(
            user.Id,
            user.Email,
            user.FullName,
            user.Role,
            user.CurrentStage,
            user.PrimaryObjective,
            user.PreferredPlatform,
            user.HoursAvailablePerWeek,
            user.SkillsAndConstraints,
            user.CreatedAt
        );

        return ApiResponse<UserProfileDto>.Ok(dto);
    }

    public async Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request, CancellationToken ct = default)
    {
        var user = await _db.Users.FindAsync(new object[] { userId }, ct);
        if (user == null)
        {
            return ApiResponse<UserProfileDto>.Fail("User not found.");
        }

        user.FullName = request.FullName?.Trim() ?? user.FullName;
        user.CurrentStage = request.CurrentStage;
        user.PrimaryObjective = request.PrimaryObjective;
        user.PreferredPlatform = request.PreferredPlatform;
        user.HoursAvailablePerWeek = Math.Clamp(request.HoursAvailablePerWeek, 1, 100);
        user.SkillsAndConstraints = request.SkillsAndConstraints?.Trim() ?? string.Empty;

        await _db.SaveChangesAsync(ct);

        var dto = new UserProfileDto(
            user.Id,
            user.Email,
            user.FullName,
            user.Role,
            user.CurrentStage,
            user.PrimaryObjective,
            user.PreferredPlatform,
            user.HoursAvailablePerWeek,
            user.SkillsAndConstraints,
            user.CreatedAt
        );

        return ApiResponse<UserProfileDto>.Ok(dto, "Profile updated successfully.");
    }
}

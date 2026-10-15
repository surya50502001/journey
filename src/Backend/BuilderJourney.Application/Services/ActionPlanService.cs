using Microsoft.EntityFrameworkCore;
using BuilderJourney.Application.DTOs;
using BuilderJourney.Application.Engines;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Domain.Entities;

namespace BuilderJourney.Application.Services;

public class ActionPlanService : IActionPlanService
{
    private readonly IAppDbContext _db;
    private readonly IPersonalizedActionPlanGenerator _planGenerator;

    public ActionPlanService(IAppDbContext db, IPersonalizedActionPlanGenerator planGenerator)
    {
        _db = db;
        _planGenerator = planGenerator;
    }

    public async Task<ApiResponse<ActionPlanDto>> GenerateActionPlanAsync(Guid userId, GenerateActionPlanRequest request, CancellationToken ct = default)
    {
        var user = await _db.Users.FindAsync(new object[] { userId }, ct);
        if (user == null)
        {
            return ApiResponse<ActionPlanDto>.Fail("User not found.");
        }

        // Create or update user goal
        var userGoal = new UserGoal
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = userId,
            ApplicationUser = user,
            Stage = request.Stage,
            Objective = request.Objective,
            HoursPerWeek = Math.Clamp(request.HoursPerWeek, 1, 100),
            Skills = request.Skills?.Trim() ?? string.Empty,
            Constraints = request.Constraints?.Trim() ?? string.Empty,
            TargetPlatform = request.TargetPlatform,
            CreatedAt = DateTime.UtcNow
        };

        _db.UserGoals.Add(userGoal);

        // Fetch reference builder profiles with milestones
        var referenceProfiles = await _db.BuilderProfiles
            .Include(b => b.JourneyEvents)
            .Where(b => b.IsCurated)
            .Take(5)
            .ToListAsync(ct);

        var plan = _planGenerator.GeneratePlan(userGoal, referenceProfiles);
        _db.ActionPlans.Add(plan);
        await _db.SaveChangesAsync(ct);

        return await MapToDtoAsync(plan.Id, ct);
    }

    public async Task<ApiResponse<ActionPlanDto>> GetLatestActionPlanAsync(Guid userId, CancellationToken ct = default)
    {
        var latestPlan = await _db.ActionPlans
            .Include(p => p.UserGoal)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.RelevantBuilderProfile)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.RelevantEvent)
            .Where(p => p.ApplicationUserId == userId)
            .OrderByDescending(p => p.GeneratedAt)
            .FirstOrDefaultAsync(ct);

        if (latestPlan == null)
        {
            // If user has no plan, generate default plan based on their user profile stage
            var user = await _db.Users.FindAsync(new object[] { userId }, ct);
            if (user == null)
            {
                return ApiResponse<ActionPlanDto>.Fail("User not found.");
            }

            return await GenerateActionPlanAsync(userId, new GenerateActionPlanRequest(
                user.CurrentStage,
                user.PrimaryObjective,
                user.HoursAvailablePerWeek,
                user.SkillsAndConstraints,
                "",
                user.PreferredPlatform
            ), ct);
        }

        return await MapToDtoAsync(latestPlan.Id, ct);
    }

    public async Task<ApiResponse<ActionPlanTaskDto>> UpdateTaskStatusAsync(Guid userId, Guid taskId, UpdateTaskStatusRequest request, CancellationToken ct = default)
    {
        var task = await _db.ActionPlanTasks
            .Include(t => t.ActionPlan)
            .Include(t => t.RelevantBuilderProfile)
            .Include(t => t.RelevantEvent)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.ActionPlan.ApplicationUserId == userId, ct);

        if (task == null)
        {
            return ApiResponse<ActionPlanTaskDto>.Fail("Task not found or access denied.");
        }

        task.IsCompleted = request.IsCompleted;
        await _db.SaveChangesAsync(ct);

        var dto = new ActionPlanTaskDto(
            task.Id,
            task.WeekNumber,
            task.FocusArea,
            task.Title,
            task.ActionDescription,
            task.SuggestedContentFormat,
            task.MilestoneTarget,
            task.RelevantBuilderProfileId,
            task.RelevantBuilderProfile?.Name,
            task.RelevantBuilderProfile?.Handle,
            task.RelevantEventId,
            task.RelevantEvent?.Title,
            task.SupportingRationale,
            task.SuccessMetric,
            task.IsCompleted
        );

        return ApiResponse<ActionPlanTaskDto>.Ok(dto, "Task status updated.");
    }

    private async Task<ApiResponse<ActionPlanDto>> MapToDtoAsync(Guid planId, CancellationToken ct)
    {
        var plan = await _db.ActionPlans
            .Include(p => p.UserGoal)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.RelevantBuilderProfile)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.RelevantEvent)
            .FirstOrDefaultAsync(p => p.Id == planId, ct);

        if (plan == null)
        {
            return ApiResponse<ActionPlanDto>.Fail("Action plan not found.");
        }

        var taskDtos = plan.Tasks
            .OrderBy(t => t.WeekNumber)
            .Select(t => new ActionPlanTaskDto(
                t.Id,
                t.WeekNumber,
                t.FocusArea,
                t.Title,
                t.ActionDescription,
                t.SuggestedContentFormat,
                t.MilestoneTarget,
                t.RelevantBuilderProfileId,
                t.RelevantBuilderProfile?.Name,
                t.RelevantBuilderProfile?.Handle,
                t.RelevantEventId,
                t.RelevantEvent?.Title,
                t.SupportingRationale,
                t.SuccessMetric,
                t.IsCompleted
            ))
            .ToList();

        var dto = new ActionPlanDto(
            plan.Id,
            plan.UserGoalId,
            plan.UserGoal.Stage,
            plan.UserGoal.Objective,
            plan.UserGoal.HoursPerWeek,
            plan.UserGoal.TargetPlatform,
            plan.Title,
            plan.Summary,
            plan.MethodologyExplanation,
            plan.GeneratedAt,
            taskDtos
        );

        return ApiResponse<ActionPlanDto>.Ok(dto);
    }
}

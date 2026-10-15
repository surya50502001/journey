using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuilderJourney.Application.Engines;
using BuilderJourney.Application.Interfaces;
using BuilderJourney.Application.Services;
using BuilderJourney.Domain.Interfaces;
using BuilderJourney.Infrastructure.Background;
using BuilderJourney.Infrastructure.Data;
using BuilderJourney.Infrastructure.Providers;
using BuilderJourney.Infrastructure.Security;

namespace BuilderJourney.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database configuration: Supports PostgreSQL as primary, with SQLite fallback for zero-install dev
        var dbProvider = configuration["DatabaseProvider"] ?? "Sqlite";
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=builder_journey.db";

        services.AddDbContext<AppDbContext>(options =>
        {
            if (dbProvider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
            {
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                });
            }
            else
            {
                options.UseSqlite(connectionString, sqlite =>
                {
                    sqlite.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                });
            }
        });

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Security
        services.AddSingleton<IEncryptionService, AesEncryptionService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // Core Intelligence Engines
        services.AddScoped<IJourneyReconstructionEngine, JourneyReconstructionEngine>();
        services.AddScoped<IContentStrategyAnalyzer, ContentStrategyAnalyzer>();
        services.AddScoped<IBreakthroughDetector, BreakthroughDetector>();
        services.AddScoped<IPersonalizedActionPlanGenerator, PersonalizedActionPlanGenerator>();
        services.AddScoped<IBuilderComparator, BuilderComparator>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBuilderService, BuilderService>();
        services.AddScoped<IJourneyService, JourneyService>();
        services.AddScoped<IContentAnalysisService, ContentAnalysisService>();
        services.AddScoped<IBreakthroughService, BreakthroughService>();
        services.AddScoped<IBuilderComparisonService, BuilderComparisonService>();
        services.AddScoped<IActionPlanService, ActionPlanService>();
        services.AddScoped<IWatchlistService, WatchlistService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IOperationsService, OperationsService>();

        // Platform Providers
        services.AddHttpClient<XPlatformDataProvider>();
        services.AddScoped<IPlatformDataProvider, XPlatformDataProvider>();
        services.AddScoped<IPlatformDataProvider, CuratedPlatformDataProvider>();

        // Database Seeder
        services.AddScoped<DatabaseSeeder>();

        // Background Job Worker
        services.AddHostedService<BackgroundSyncWorker>();

        return services;
    }
}

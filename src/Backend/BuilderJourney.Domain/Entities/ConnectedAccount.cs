using BuilderJourney.Domain.Enums;

namespace BuilderJourney.Domain.Entities;

public class ConnectedAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public PlatformType Platform { get; set; }
    public string ProviderUserId { get; set; } = string.Empty;
    public string ProviderUsername { get; set; } = string.Empty;
    public string EncryptedAccessToken { get; set; } = string.Empty;
    public string? EncryptedRefreshToken { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public string Scopes { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSyncedAt { get; set; }
}

public class PlatformProvider
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public PlatformType Platform { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string SupportedMetricsSummary { get; set; } = string.Empty;
    public string LimitationsNotice { get; set; } = string.Empty;
}

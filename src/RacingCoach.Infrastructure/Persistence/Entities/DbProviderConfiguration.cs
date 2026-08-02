namespace RacingCoach.Infrastructure.Persistence.Entities;

internal class DbProviderConfiguration
{
    public Guid Id { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SettingsJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

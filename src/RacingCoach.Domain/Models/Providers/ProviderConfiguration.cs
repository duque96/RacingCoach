namespace RacingCoach.Domain.Models.Providers;

public class ProviderConfiguration
{
    public Guid Id { get; private set; }
    public string ProviderId { get; private set; }
    public string Name { get; private set; }
    public Dictionary<string, string> Settings { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ProviderConfiguration()
    {
        ProviderId = string.Empty;
        Name = string.Empty;
        Settings = new Dictionary<string, string>();
    }

    public ProviderConfiguration(string providerId, string name, Dictionary<string, string> settings)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Configuration name cannot be empty", nameof(name));
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        Id = Guid.NewGuid();
        ProviderId = providerId;
        Name = name;
        Settings = new Dictionary<string, string>(settings);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, Dictionary<string, string> settings)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Configuration name cannot be empty", nameof(name));
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        Name = name;
        Settings = new Dictionary<string, string>(settings);
        UpdatedAt = DateTime.UtcNow;
    }

    public string? GetSetting(string key)
    {
        return Settings.TryGetValue(key, out var value) ? value : null;
    }

    public int GetIntSetting(string key, int defaultValue = 0)
    {
        var value = GetSetting(key);
        return value != null && int.TryParse(value, out var result) ? result : defaultValue;
    }

    public bool GetBoolSetting(string key, bool defaultValue = false)
    {
        var value = GetSetting(key);
        return value != null && bool.TryParse(value, out var result) ? result : defaultValue;
    }
}

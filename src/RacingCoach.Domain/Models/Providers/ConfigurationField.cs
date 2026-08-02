namespace RacingCoach.Domain.Models.Providers;

public class ConfigurationField
{
    public string Name { get; }
    public string DisplayName { get; }
    public string Type { get; }
    public bool Required { get; }
    public string? DefaultValue { get; }
    public string? ValidationPattern { get; }
    public string? Description { get; }

    public ConfigurationField(
        string name,
        string displayName,
        string type,
        bool required = false,
        string? defaultValue = null,
        string? validationPattern = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty", nameof(displayName));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Field type cannot be empty", nameof(type));

        Name = name;
        DisplayName = displayName;
        Type = type;
        Required = required;
        DefaultValue = defaultValue;
        ValidationPattern = validationPattern;
        Description = description;
    }
}

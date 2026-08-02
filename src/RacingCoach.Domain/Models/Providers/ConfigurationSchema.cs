namespace RacingCoach.Domain.Models.Providers;

public class ConfigurationSchema
{
    private readonly List<ConfigurationField> _fields = new();

    public IReadOnlyList<ConfigurationField> Fields => _fields.AsReadOnly();

    public ConfigurationSchema()
    {
    }

    public ConfigurationSchema(IEnumerable<ConfigurationField> fields)
    {
        _fields.AddRange(fields);
    }

    public ConfigurationSchema AddField(ConfigurationField field)
    {
        _fields.Add(field);
        return this;
    }

    public ConfigurationSchema AddStringField(
        string name,
        string displayName,
        bool required = false,
        string? defaultValue = null,
        string? validationPattern = null,
        string? description = null)
    {
        return AddField(new ConfigurationField(
            name,
            displayName,
            "string",
            required,
            defaultValue,
            validationPattern,
            description));
    }

    public ConfigurationSchema AddIntField(
        string name,
        string displayName,
        bool required = false,
        string? defaultValue = null,
        string? description = null)
    {
        return AddField(new ConfigurationField(
            name,
            displayName,
            "int",
            required,
            defaultValue,
            null,
            description));
    }

    public ConfigurationSchema AddBoolField(
        string name,
        string displayName,
        bool required = false,
        string? defaultValue = null,
        string? description = null)
    {
        return AddField(new ConfigurationField(
            name,
            displayName,
            "bool",
            required,
            defaultValue,
            null,
            description));
    }
}

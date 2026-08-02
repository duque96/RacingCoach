using RacingCoach.Domain.Models.Providers;

namespace RacingCoach.Domain.Interfaces;

public interface ITelemetryProvider
{
    string Id { get; }
    string Name { get; }
    string GameName { get; }
    ConfigurationSchema ConfigurationSchema { get; }
    ITelemetryParser CreateParser();
    ITelemetryListener CreateListener(ProviderConfiguration config);
}

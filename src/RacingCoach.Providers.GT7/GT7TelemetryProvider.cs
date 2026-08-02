using RacingCoach.Domain.Interfaces;
using RacingCoach.Domain.Models.Providers;

namespace RacingCoach.Providers.GT7;

internal class GT7TelemetryProvider : ITelemetryProvider
{
    public string Id => "gt7";
    public string Name => "Gran Turismo 7";
    public string GameName => "Gran Turismo 7";

    public ConfigurationSchema ConfigurationSchema => new ConfigurationSchema()
        .AddStringField(
            name: "PlaystationIP",
            displayName: "PlayStation IP Address",
            required: true,
            validationPattern: @"^(\d{1,3}\.){3}\d{1,3}$",
            description: "IP address of the PlayStation 5 console")
        .AddIntField(
            name: "Port",
            displayName: "UDP Port",
            required: false,
            defaultValue: "33740",
            description: "UDP port to listen for telemetry data");

    public ITelemetryParser CreateParser()
    {
        return new GT7TelemetryParser();
    }

    public ITelemetryListener CreateListener(ProviderConfiguration config)
    {
        return new GT7UdpListener(config);
    }
}

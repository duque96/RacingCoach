using RacingCoach.Domain.Common;
using RacingCoach.Domain.Models.Telemetry;

namespace RacingCoach.Domain.Interfaces;

public interface ITelemetryParser
{
    string GameName { get; }
    Result<TelemetryData> Parse(byte[] data);
}

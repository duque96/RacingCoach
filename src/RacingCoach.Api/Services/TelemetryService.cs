using RacingCoach.Domain.Interfaces;
using RacingCoach.Domain.Models.Telemetry;

namespace RacingCoach.Api.Services;

public class TelemetryService
{
    private readonly ITelemetryDataRepository _telemetryDataRepository;
    private TelemetryData? _lastTelemetry;
    private DateTime _lastUpdateTime;

    public TelemetryService(ITelemetryDataRepository telemetryDataRepository)
    {
        _telemetryDataRepository = telemetryDataRepository;
        _lastUpdateTime = DateTime.MinValue;
    }

    public TelemetryData? LastTelemetry => _lastTelemetry;

    public async Task UpdateAsync()
    {
        var recentData = await _telemetryDataRepository.GetRecentAsync(1);
        if (!recentData.IsSuccess || !recentData.Value.Any())
            return;

        var telemetry = recentData.Value.First();
        
        if (telemetry.Timestamp <= _lastUpdateTime)
            return;

        _lastTelemetry = telemetry;
        _lastUpdateTime = telemetry.Timestamp;
    }
}

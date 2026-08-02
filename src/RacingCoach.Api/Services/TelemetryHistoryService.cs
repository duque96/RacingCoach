using System.Collections.Concurrent;
using RacingCoach.Domain.Interfaces;

namespace RacingCoach.Api.Services;

public class TelemetryHistoryService
{
    private readonly ITelemetryDataRepository _telemetryDataRepository;
    private readonly ConcurrentQueue<TelemetryDataPoint> _history = new();
    private const int MaxDataPoints = 100;
    private DateTime _lastUpdateTime;

    public TelemetryHistoryService(ITelemetryDataRepository telemetryDataRepository)
    {
        _telemetryDataRepository = telemetryDataRepository;
        _lastUpdateTime = DateTime.MinValue;
    }

    public IReadOnlyList<TelemetryDataPoint> History => _history.ToArray();

    public async Task UpdateAsync()
    {
        var recentData = await _telemetryDataRepository.GetRecentAsync(1);
        if (!recentData.IsSuccess || !recentData.Value.Any())
            return;

        var telemetry = recentData.Value.First();

        if (telemetry.Timestamp <= _lastUpdateTime)
            return;

        var dataPoint = new TelemetryDataPoint
        {
            Timestamp = telemetry.Timestamp,
            Speed = telemetry.Speed,
            Throttle = telemetry.Throttle,
            Brake = telemetry.Brake,
            Gear = telemetry.Gear,
            RPM = telemetry.RPM
        };

        _history.Enqueue(dataPoint);

        while (_history.Count > MaxDataPoints)
        {
            _history.TryDequeue(out _);
        }

        _lastUpdateTime = telemetry.Timestamp;
    }
}

public class TelemetryDataPoint
{
    public DateTime Timestamp { get; set; }
    public double Speed { get; set; }
    public double Throttle { get; set; }
    public double Brake { get; set; }
    public int Gear { get; set; }
    public double RPM { get; set; }
}

namespace RacingCoach.Domain.Interfaces;

public interface ITelemetryListener
{
    bool IsListening { get; }
    event Action<byte[]>? OnDataReceived;
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync();
}

using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using RacingCoach.Domain.Interfaces;
using RacingCoach.Domain.Models.Providers;

namespace RacingCoach.Providers.GT7;

internal class GT7UdpListener : ITelemetryListener, IDisposable
{
    private readonly ProviderConfiguration _config;
    private readonly ILogger<GT7UdpListener>? _logger;
    private UdpClient? _udpClient;
    private Timer? _heartbeatTimer;
    private DateTime _lastPacketReceived;
    private CancellationTokenSource? _cts;
    private Task? _receiveTask;

    public bool IsListening { get; private set; }
    public event Action<byte[]>? OnDataReceived;

    public GT7UdpListener(ProviderConfiguration config, ILogger<GT7UdpListener>? logger = null)
    {
        _config = config;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (IsListening)
            return Task.CompletedTask;

        var port = _config.GetIntSetting("Port", 33740);
        var playstationIp = _config.GetSetting("PlaystationIP") ?? string.Empty;

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _udpClient = new UdpClient(port);
        _lastPacketReceived = DateTime.UtcNow;
        IsListening = true;

        _heartbeatTimer = new Timer(SendHeartbeat, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        _receiveTask = Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync(_cts.Token);
                    _lastPacketReceived = DateTime.UtcNow;

                    if (result.Buffer.Length == 0)
                    {
                        _logger?.LogWarning("Received empty UDP packet");
                        continue;
                    }

                    _logger?.LogDebug("Received UDP packet of {Length} bytes from {EndPoint}",
                        result.Buffer.Length, result.RemoteEndPoint);

                    OnDataReceived?.Invoke(result.Buffer);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error receiving UDP packet");
                    await Task.Delay(100, _cts.Token);
                }
            }
        }, _cts.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (!IsListening)
            return;

        IsListening = false;

        _cts?.Cancel();

        if (_receiveTask != null)
        {
            try
            {
                await _receiveTask;
            }
            catch (OperationCanceledException)
            {
            }
        }

        _heartbeatTimer?.Dispose();
        _heartbeatTimer = null;

        _udpClient?.Dispose();
        _udpClient = null;

        _cts?.Dispose();
        _cts = null;
    }

    private void SendHeartbeat(object? state)
    {
        if (_udpClient == null)
            return;

        var playstationIp = _config.GetSetting("PlaystationIP");
        if (string.IsNullOrEmpty(playstationIp))
            return;

        var timeSinceLastPacket = DateTime.UtcNow - _lastPacketReceived;

        if (timeSinceLastPacket > TimeSpan.FromSeconds(10))
        {
            _logger?.LogWarning("No packets received for {Seconds} seconds, reconnecting",
                timeSinceLastPacket.TotalSeconds);

            try
            {
                var port = _config.GetIntSetting("Port", 33740);
                _udpClient.Close();
                _udpClient = new UdpClient(port);
                _lastPacketReceived = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error reconnecting UDP client");
            }
        }

        try
        {
            var heartbeatData = new byte[] { 0x41 };
            _udpClient.Send(heartbeatData, heartbeatData.Length, playstationIp, 33739);
            _logger?.LogDebug("Sent heartbeat to {IP}:33739", playstationIp);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error sending heartbeat");
        }
    }

    public void Dispose()
    {
        _heartbeatTimer?.Dispose();
        _udpClient?.Dispose();
        _cts?.Dispose();
    }
}

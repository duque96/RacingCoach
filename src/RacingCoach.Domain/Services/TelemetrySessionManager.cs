using RacingCoach.Domain.Common;
using RacingCoach.Domain.Interfaces;
using RacingCoach.Domain.Models.Sessions;
using RacingCoach.Domain.Models.Telemetry;
using RacingCoach.Domain.Models.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace RacingCoach.Domain.Services;

public class TelemetrySessionManager
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEnumerable<ITelemetryProvider> _providers;
    
    private ITelemetryListener? _activeListener;
    private GameSession? _activeSession;
    private CancellationTokenSource? _listenerCts;

    public TelemetrySessionManager(
        IServiceScopeFactory scopeFactory,
        IEnumerable<ITelemetryProvider> providers)
    {
        _scopeFactory = scopeFactory;
        _providers = providers;
    }

    public GameSession? ActiveSession => _activeSession;

    public async Task<Result<GameSession>> StartSessionAsync(string providerId, Guid configurationId)
    {
        if (_activeSession != null)
            return Result<GameSession>.Failure(Error.Validation("A session is already active"));

        var provider = _providers.FirstOrDefault(p => p.Id == providerId);
        if (provider == null)
            return Result<GameSession>.Failure(Error.NotFound($"Provider '{providerId}' not found"));

        ProviderConfiguration config;
        using (var scope = _scopeFactory.CreateScope())
        {
            var configRepository = scope.ServiceProvider.GetRequiredService<IProviderConfigurationRepository>();
            var configResult = await configRepository.GetByIdAsync(configurationId);
            if (!configResult.IsSuccess)
                return Result<GameSession>.Failure(configResult.Error);

            config = configResult.Value;
            if (config.ProviderId != providerId)
                return Result<GameSession>.Failure(Error.Validation("Configuration does not belong to the specified provider"));

            var sessionRepository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();
            var session = new GameSession(provider.GameName);
            var sessionResult = await sessionRepository.AddAsync(session);
            if (!sessionResult.IsSuccess)
                return Result<GameSession>.Failure(sessionResult.Error);

            var getSessionResult = await sessionRepository.GetByIdAsync(sessionResult.Value);
            if (!getSessionResult.IsSuccess)
                return Result<GameSession>.Failure(getSessionResult.Error);

            _activeSession = getSessionResult.Value;
        }

        _activeListener = provider.CreateListener(config);
        _activeListener.OnDataReceived += async data => await HandleTelemetryDataAsync(data, provider);

        _listenerCts = new CancellationTokenSource();
        await _activeListener.StartAsync(_listenerCts.Token);

        return Result<GameSession>.Success(_activeSession);
    }

    public async Task<Result> StopSessionAsync()
    {
        if (_activeSession == null)
            return Result.Failure(Error.Validation("No active session"));

        if (_activeListener != null)
        {
            _listenerCts?.Cancel();
            await _activeListener.StopAsync();
            _activeListener.OnDataReceived -= async data => await HandleTelemetryDataAsync(data, null!);
            _activeListener = null;
        }

        _listenerCts?.Dispose();
        _listenerCts = null;

        using (var scope = _scopeFactory.CreateScope())
        {
            var sessionRepository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();
            _activeSession.End();
            await sessionRepository.UpdateAsync(_activeSession);
        }

        _activeSession = null;

        return Result.Success();
    }

    private async Task HandleTelemetryDataAsync(byte[] data, ITelemetryProvider provider)
    {
        if (_activeSession == null)
            return;

        try
        {
            var parser = provider.CreateParser();
            var parseResult = parser.Parse(data);

            if (!parseResult.IsSuccess)
                return;

            var telemetry = parseResult.Value;
            var telemetryData = telemetry.WithSessionId(_activeSession.Id);
            
            using (var scope = _scopeFactory.CreateScope())
            {
                var telemetryDataRepository = scope.ServiceProvider.GetRequiredService<ITelemetryDataRepository>();
                await telemetryDataRepository.AddAsync(telemetryData);
            }
        }
        catch
        {
            // Log error but don't crash the listener
        }
    }
}

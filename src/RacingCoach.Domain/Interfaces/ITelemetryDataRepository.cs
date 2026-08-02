using RacingCoach.Domain.Common;
using RacingCoach.Domain.Models.Telemetry;

namespace RacingCoach.Domain.Interfaces;

public interface ITelemetryDataRepository
{
    Task<Result<TelemetryData>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TelemetryData>>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TelemetryData>>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
    Task<Result<Guid>> AddAsync(TelemetryData telemetryData, CancellationToken cancellationToken = default);
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);
    Task<Result<int>> GetCountBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
}

using RacingCoach.Domain.Common;
using RacingCoach.Domain.Models.Sessions;

namespace RacingCoach.Domain.Interfaces;

public interface ISessionRepository
{
    Task<Result<GameSession>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<GameSession>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<GameSession>>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Result<Guid>> AddAsync(GameSession session, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(GameSession session, CancellationToken cancellationToken = default);
}

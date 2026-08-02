using Microsoft.EntityFrameworkCore;
using RacingCoach.Domain.Common;
using RacingCoach.Domain.Interfaces;
using RacingCoach.Domain.Models.Sessions;
using RacingCoach.Infrastructure.Persistence;

namespace RacingCoach.Infrastructure.Persistence.Repositories;

internal class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GameSession>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await _context.GameSessions
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return session is null
            ? Result<GameSession>.Failure(Error.NotFound($"Session {id} not found"))
            : Result<GameSession>.Success(session);
    }

    public async Task<Result<IEnumerable<GameSession>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await _context.GameSessions
            .OrderByDescending(s => s.StartTime)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<GameSession>>.Success(sessions);
    }

    public async Task<Result<IEnumerable<GameSession>>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await _context.GameSessions
            .Where(s => s.EndTime == null)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<GameSession>>.Success(sessions);
    }

    public async Task<Result<Guid>> AddAsync(GameSession session, CancellationToken cancellationToken = default)
    {
        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(session.Id);
    }

    public async Task<Result> UpdateAsync(GameSession session, CancellationToken cancellationToken = default)
    {
        _context.GameSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RacingCoach.Domain.Models.Sessions;
using RacingCoach.Infrastructure.Persistence;
using RacingCoach.Infrastructure.Persistence.Repositories;

namespace RacingCoach.Tests.Integration.Infrastructure.Repositories;

public class SessionRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly SessionRepository _repository;

    public SessionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new SessionRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidSession_ShouldPersist()
    {
        var session = new GameSession("Gran Turismo 7");

        var result = await _repository.AddAsync(session);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(session.Id);

        var count = await _context.GameSessions.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingSession_ShouldReturnSession()
    {
        var session = new GameSession("Gran Turismo 7");
        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(session.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.GameName.Should().Be("Gran Turismo 7");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingSession_ShouldReturnFailure()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("NotFound");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSessions()
    {
        _context.GameSessions.Add(new GameSession("GT7"));
        _context.GameSessions.Add(new GameSession("F1 25"));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetActiveAsync_ShouldReturnOnlyActiveSessions()
    {
        var activeSession = new GameSession("GT7");
        var endedSession = new GameSession("F1 25");
        endedSession.End();

        _context.GameSessions.AddRange(activeSession, endedSession);
        await _context.SaveChangesAsync();

        var result = await _repository.GetActiveAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().GameName.Should().Be("GT7");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

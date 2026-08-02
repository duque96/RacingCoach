using FluentAssertions;
using RacingCoach.Domain.Models.Sessions;

namespace RacingCoach.Tests.Unit.Domain.Models;

public class GameSessionTests
{
    [Fact]
    public void Constructor_ValidGameName_ShouldCreateSession()
    {
        var session = new GameSession("Gran Turismo 7");

        session.Id.Should().NotBeEmpty();
        session.GameName.Should().Be("Gran Turismo 7");
        session.StartTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        session.EndTime.Should().BeNull();
        session.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Constructor_EmptyGameName_ShouldThrowArgumentException()
    {
        var act = () => new GameSession("");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Game name cannot be empty*");
    }

    [Fact]
    public void Constructor_NullGameName_ShouldThrowArgumentException()
    {
        var act = () => new GameSession(null!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void End_ActiveSession_ShouldSetEndTime()
    {
        var session = new GameSession("Gran Turismo 7");

        session.End();

        session.EndTime.Should().NotBeNull();
        session.EndTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        session.IsActive.Should().BeFalse();
    }

    [Fact]
    public void End_AlreadyEndedSession_ShouldThrowInvalidOperationException()
    {
        var session = new GameSession("Gran Turismo 7");
        session.End();

        var act = () => session.End();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Session already ended");
    }
}

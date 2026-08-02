namespace RacingCoach.Domain.Models.Sessions;

public class GameSession
{
    public Guid Id { get; private set; }
    public string GameName { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public bool IsActive => EndTime is null;

    private GameSession()
    {
        GameName = string.Empty;
    }

    public GameSession(string gameName)
    {
        if (string.IsNullOrWhiteSpace(gameName))
            throw new ArgumentException("Game name cannot be empty", nameof(gameName));

        Id = Guid.NewGuid();
        GameName = gameName;
        StartTime = DateTime.UtcNow;
    }

    public void End()
    {
        if (EndTime is not null)
            throw new InvalidOperationException("Session already ended");

        EndTime = DateTime.UtcNow;
    }
}

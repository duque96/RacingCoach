namespace RacingCoach.Shared.Enums;

public enum GameType
{
    Unknown = 0,
    GranTurismo7 = 1,
    F1_25 = 2,
    AssettoCorsa = 3,
    IRLRacing = 4
}

public static class GameTypeExtensions
{
    public static string ToDisplayName(this GameType gameType) => gameType switch
    {
        GameType.GranTurismo7 => "Gran Turismo 7",
        GameType.F1_25 => "F1 25",
        GameType.AssettoCorsa => "Assetto Corsa",
        GameType.IRLRacing => "iRacing",
        _ => "Unknown"
    };
}

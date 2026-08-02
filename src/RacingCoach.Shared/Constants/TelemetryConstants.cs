namespace RacingCoach.Shared.Constants;

public static class TelemetryConstants
{
    public const int DefaultUdpPort = 3333;
    public const int MaxPacketSize = 4096;
    public const int MinPacketSize = 32;
    public const int DefaultRecentCount = 100;

    public static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(30);
}

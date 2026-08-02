# RacingCoach.Shared - Conventions

## Responsibility

Transversal layer for shared elements. Contains:
- DTOs shared between layers
- Global constants
- Transversal helpers and extension methods
- Shared enums
- Common validations

## Dependencies

- Does **NOT** depend on any other project layer
- Can be referenced by all layers
- No business logic

## Folder Structure

```
Shared/
├── DTOs/                # Data Transfer Objects
│   ├── Telemetry/
│   │   └── TelemetryDataDto.cs
│   └── Sessions/
│       └── SessionSummaryDto.cs
├── Constants/           # Global constants
│   ├── GameNames.cs
│   └── TelemetryConstants.cs
├── Enums/               # Shared enums
│   └── GameType.cs
├── Extensions/          # Extension methods
│   └── StringExtensions.cs
└── Helpers/             # Transversal helpers
    └── DateTimeHelper.cs
```

## Specific Conventions

### DTOs
- Records for immutability
- No business logic
- Only public properties
- Basic validations if necessary

```csharp
public record SessionSummaryDto(
    Guid Id,
    string GameName,
    DateTime StartTime,
    DateTime? EndTime,
    int PacketCount);

public record TelemetryDataDto(
    DateTime Timestamp,
    double Speed,
    double Rpm,
    int Gear,
    double Throttle,
    double Brake);
```

### Constants
- Static classes with related constants
- Group by domain/topic
- Use `const` for compile-time known values
- Use `static readonly` for runtime calculated values

```csharp
public static class GameNames
{
    public const string GranTurismo7 = "Gran Turismo 7";
    public const string F1_25 = "F1 25";
    public const string AssettoCorsa = "Assetto Corsa";
}

public static class TelemetryConstants
{
    public const int DefaultUdpPort = 3333;
    public const int MaxPacketSize = 4096;
    public const int MinPacketSize = 32;
    
    public static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(30);
}
```

### Enums
- Enums for discrete and known values
- Use in DTOs and domain models
- Provide extension methods for conversions

```csharp
public enum GameType
{
    Unknown = 0,
    GranTurismo7 = 1,
    F1_25 = 2,
    AssettoCorsa = 3
}

public static class GameTypeExtensions
{
    public static string ToDisplayName(this GameType gameType) => gameType switch
    {
        GameType.GranTurismo7 => "Gran Turismo 7",
        GameType.F1_25 => "F1 25",
        GameType.AssettoCorsa => "Assetto Corsa",
        _ => "Unknown"
    };
}
```

### Extension Methods
- Extension methods for common types
- No business logic dependencies
- Reusable across the project

```csharp
public static class DateTimeExtensions
{
    public static string ToDisplayString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public static bool IsWithinLast(this DateTime dateTime, TimeSpan duration)
    {
        return dateTime > DateTime.UtcNow.Subtract(duration);
    }
}
```

### Helpers
- Static classes with utility methods
- Stateless
- Pure (same inputs = same outputs)

```csharp
public static class MathHelper
{
    public static double Clamp(double value, double min, double max)
    {
        return Math.Max(min, Math.Min(max, value));
    }
    
    public static double Lerp(double a, double b, double t)
    {
        return a + (b - a) * Clamp(t, 0.0, 1.0);
    }
}
```

## Usage in Other Layers

### In Domain
```csharp
using RacingCoach.Shared.Constants;
using RacingCoach.Shared.Enums;

public class GameSession
{
    public GameType GameType { get; private set; }
    
    public GameSession(GameType gameType)
    {
        GameType = gameType;
    }
}
```

### In Infrastructure
```csharp
using RacingCoach.Shared.DTOs;

public class SessionRepository : ISessionRepository
{
    public async Task<SessionSummaryDto> GetSummaryAsync(Guid id)
    {
        // Map to Shared DTO
        return new SessionSummaryDto(...);
    }
}
```

### In Api
```csharp
using RacingCoach.Shared.DTOs;
using RacingCoach.Shared.Extensions;

public class SessionEndpoints
{
    public static async Task<IResult> GetSession(Guid id, ISessionRepository repository)
    {
        var session = await repository.GetByIdAsync(id);
        var dto = new SessionSummaryDto(...);
        return Results.Ok(dto);
    }
}
```

## Principles

1. **No business logic:** Only data, constants, and helpers
2. **Immutability:** Prefer records and immutable types
3. **Reusability:** Elements used by multiple layers
4. **No dependencies:** Don't reference Domain, Infrastructure, or Api
5. **Strong typing:** Avoid object and dynamic

## Anti-patterns to Avoid

- Business logic in Shared
- Dependencies on other project layers
- DTOs with behavior
- Hardcoded constants in multiple places
- Extension methods that modify state
- Helpers with external dependencies

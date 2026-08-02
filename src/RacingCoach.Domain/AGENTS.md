# RacingCoach.Domain - Conventions

## Responsibility

Core business layer. Contains:
- Domain models
- Repository interfaces
- Commands and handlers (Command + Handler Pattern)
- Specifications and business rules
- Value Objects

## Dependencies

- Does **NOT** depend on any other layer
- It is the core of the architecture
- Only references to Shared (basic DTOs, constants)

## Folder Structure

```
Domain/
├── Models/              # Entities and value objects
│   ├── Telemetry/
│   │   └── TelemetryData.cs
│   ├── Sessions/
│   │   └── GameSession.cs
│   └── Providers/
│       ├── ProviderConfiguration.cs
│       ├── ConfigurationSchema.cs
│       └── ConfigurationField.cs
├── Interfaces/          # Repository contracts
│   ├── ISessionRepository.cs
│   ├── ITelemetryDataRepository.cs
│   ├── IProviderConfigurationRepository.cs
│   ├── ITelemetryParser.cs
│   ├── ITelemetryProvider.cs
│   └── ITelemetryListener.cs
├── Services/            # Domain services
│   └── TelemetrySessionManager.cs
├── Commands/            # Commands and handlers (future)
├── Events/              # Domain events (future)
└── Exceptions/          # Domain exceptions (future)
```

## Specific Conventions

### Domain Models
- Classes with behavior, not just data
- Properties with private setters (controlled immutability)
- Methods that reflect business operations
- Validations in constructors and methods

```csharp
public class GameSession
{
    public Guid Id { get; private set; }
    public string GameName { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    
    private readonly List<RawTelemetryPacket> _packets = new();
    public IReadOnlyCollection<RawTelemetryPacket> Packets => _packets.AsReadOnly();
    
    private GameSession() { } // For EF Core
    
    public GameSession(string gameName)
    {
        if (string.IsNullOrWhiteSpace(gameName))
            throw new ArgumentException("Game name cannot be empty", nameof(gameName));
        
        Id = Guid.NewGuid();
        GameName = gameName;
        StartTime = DateTime.UtcNow;
    }
    
    public void AddPacket(RawTelemetryPacket packet)
    {
        _packets.Add(packet);
    }
    
    public void End()
    {
        EndTime = DateTime.UtcNow;
    }
}
```

### Repository Interfaces
- Define only what the domain needs
- Methods that reflect business operations, not technical queries
- Return `Result<T>` for error handling

```csharp
public interface IRawPacketRepository
{
    Task<Result<RawTelemetryPacket>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<RawTelemetryPacket>>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Result<Guid>> AddAsync(RawTelemetryPacket packet, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

### Commands and Handlers
- Each command is an immutable record
- Handler implements business logic
- Always return `Result<T>`
- One command per file, handler in the same file or separate

```csharp
// Command
public record CreateSessionCommand(string GameName) : ICommand<Result<Guid>>;

// Handler
public class CreateSessionHandler : ICommandHandler<CreateSessionCommand, Result<Guid>>
{
    private readonly ISessionRepository _repository;
    
    public CreateSessionHandler(ISessionRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<Guid>> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
    {
        var session = new GameSession(command.GameName);
        var result = await _repository.AddAsync(session, cancellationToken);
        
        return result.IsSuccess 
            ? Result<Guid>.Success(session.Id) 
            : Result<Guid>.Failure(result.Error);
    }
}
```

### Result Pattern
- Use `Result<T>` for operations that can fail
- Don't throw exceptions for normal flow
- Include detailed error information

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public Error Error { get; }
    
    private Result(bool isSuccess, T value, Error error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
    
    public static Result<T> Success(T value) => new(true, value, Error.None);
    public static Result<T> Failure(Error error) => new(false, default!, error);
}

public class Error
{
    public static readonly Error None = new(string.Empty, string.Empty);
    
    public string Code { get; }
    public string Message { get; }
    
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }
    
    public static Error NotFound(string message) => new("NotFound", message);
    public static Error Validation(string message) => new("Validation", message);
    public static Error Conflict(string message) => new("Conflict", message);
}
```

### Value Objects
- Immutable
- Equality based on values, not reference
- Validations in constructor

```csharp
public record TelemetryPosition(double X, double Y, double Z)
{
    public double DistanceTo(TelemetryPosition other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        var dz = Z - other.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
}
```

## Dependency Injection

Extension method `AddDomain()` to register this layer's services:

```csharp
public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Register command handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainServiceCollectionExtensions).Assembly));
        
        return services;
    }
}
```

## Anti-patterns to Avoid

- Dependencies on Infrastructure (EF Core, SQLite, etc.)
- Presentation or UI logic
- Direct database access
- Exceptions for normal control flow
- Anemic models (only data, no behavior)
- Violation of immutability in value objects

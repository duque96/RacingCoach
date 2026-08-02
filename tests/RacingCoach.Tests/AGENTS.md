# RacingCoach.Tests - Conventions

## Responsibility

Testing project containing:
- Unit tests for handlers and commands
- Integration tests for repositories
- Telemetry parser tests
- API endpoint tests

## Dependencies

- Depends on **Domain** (to test handlers and models)
- Depends on **Infrastructure** (to test repositories and parsers)
- Depends on **Api** (to test endpoints)
- Depends on **Shared** (for DTOs and constants)

## Testing Stack

- **xUnit:** Testing framework
- **FluentAssertions:** Readable assertions
- **NSubstitute:** Mocking and stubbing
- **Microsoft.AspNetCore.Mvc.Testing:** API integration tests

## Folder Structure

```
Tests/
├── Unit/                    # Unit tests
│   ├── Domain/
│   │   ├── Handlers/
│   │   │   └── CreateSessionHandlerTests.cs
│   │   └── Models/
│   │       └── GameSessionTests.cs
│   ├── Infrastructure/
│   │   └── Parsers/
│   │       └── GT7TelemetryParserTests.cs
│   └── Shared/
│       └── Helpers/
│           └── MathHelperTests.cs
├── Integration/             # Integration tests
│   ├── Infrastructure/
│   │   └── Repositories/
│   │       └── RawPacketRepositoryTests.cs
│   └── Api/
│       └── Endpoints/
│           └── SessionEndpointsTests.cs
├── Fixtures/                # Test fixtures and test data
│   ├── TelemetryData/
│   │   └── GT7SamplePacket.bin
│   └── TestDbContextFactory.cs
└── Helpers/                 # Test helpers
    └── TestDataBuilder.cs
```

## Naming Conventions

### Test Classes
- Name as `{TestedClass}Tests`
- `Tests` suffix mandatory
- Same folder structure as tested code

```csharp
public class CreateSessionHandlerTests { }
public class GT7TelemetryParserTests { }
public class RawPacketRepositoryTests { }
```

### Test Methods
- Pattern: `Should_{ExpectedBehavior}_When_{Condition}`
- Or: `MethodName_StateUnderTest_ExpectedBehavior`
- Descriptive name explaining what is being tested

```csharp
[Fact]
public async Task Handle_ValidCommand_ShouldCreateSession()
{
    // Arrange
    // Act
    // Assert
}

[Fact]
public async Task Handle_EmptyGameName_ShouldReturnValidationError()
{
    // Arrange
    // Act
    // Assert
}
```

## Testing Patterns

### Arrange-Act-Assert (AAA)
- Clearly separate the three phases
- One phase per line or block
- Optional comments if not obvious

```csharp
[Fact]
public async Task Handle_ValidCommand_ShouldReturnSessionId()
{
    // Arrange
    var handler = new CreateSessionHandler(_repository);
    var command = new CreateSessionCommand("Gran Turismo 7");
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
}
```

### FluentAssertions
- Use fluid and readable assertions
- Descriptive error messages
- Chain assertions when it makes sense

```csharp
result.IsSuccess.Should().BeTrue();
result.Value.Should().NotBeEmpty();
result.Value.Should().BeOfType<Guid>();

session.GameName.Should().Be("Gran Turismo 7");
session.StartTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
```

### NSubstitute for Mocks
- Create interface mocks with NSubstitute
- Configure behavior with `Returns`
- Verify calls with `Received()`

```csharp
[Fact]
public async Task Handle_ShouldCallRepositoryAdd()
{
    // Arrange
    var repository = Substitute.For<ISessionRepository>();
    repository.AddAsync(Arg.Any<GameSession>(), Arg.Any<CancellationToken>())
        .Returns(Result<Guid>.Success(Guid.NewGuid()));
    
    var handler = new CreateSessionHandler(repository);
    var command = new CreateSessionCommand("Gran Turismo 7");
    
    // Act
    await handler.Handle(command, CancellationToken.None);
    
    // Assert
    await repository.Received(1).AddAsync(
        Arg.Is<GameSession>(s => s.GameName == "Gran Turismo 7"),
        Arg.Any<CancellationToken>());
}
```

## Test Types

### Unit Tests
- Test an isolated class/method
- Mock dependencies
- Fast and deterministic
- No access to external resources (DB, network, etc.)

```csharp
[Fact]
public void Parse_ValidGT7Packet_ShouldReturnNormalizedTelemetry()
{
    // Arrange
    var parser = new GT7TelemetryParser();
    var data = GetSampleGT7Packet();
    
    // Act
    var result = parser.Parse(data);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Speed.Should().BeGreaterThan(0);
}
```

### Integration Tests
- Test multiple components working together
- Use real or in-memory database
- No mocks of internal components
- Slower but more realistic

```csharp
[Fact]
public async Task AddAsync_ValidPacket_ShouldPersistToDatabase()
{
    // Arrange
    var context = TestDbContextFactory.CreateInMemoryContext();
    var repository = new RawPacketRepository(context);
    var packet = new RawTelemetryPacket(Guid.NewGuid(), DateTime.UtcNow, new byte[] { 1, 2, 3 });
    
    // Act
    var result = await repository.AddAsync(packet);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    
    var retrieved = await repository.GetByIdAsync(packet.Id);
    retrieved.IsSuccess.Should().BeTrue();
    retrieved.Value.Id.Should().Be(packet.Id);
}
```

### Endpoint Tests
- Use `WebApplicationFactory` for API
- Test complete HTTP flow
- Verify status codes and responses

```csharp
[Fact]
public async Task GetSession_ExistingId_ShouldReturnOk()
{
    // Arrange
    var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();
    var sessionId = Guid.NewGuid();
    
    // Act
    var response = await client.GetAsync($"/api/sessions/{sessionId}");
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

## Test Data and Fixtures

### TestDataBuilder
- Builder pattern to create test data
- Reasonable default values
- Fluent methods to customize

```csharp
public class GameSessionBuilder
{
    private string _gameName = "Gran Turismo 7";
    private DateTime _startTime = DateTime.UtcNow;
    
    public GameSessionBuilder WithGameName(string gameName)
    {
        _gameName = gameName;
        return this;
    }
    
    public GameSessionBuilder WithStartTime(DateTime startTime)
    {
        _startTime = startTime;
        return this;
    }
    
    public GameSession Build()
    {
        return new GameSession(_gameName);
    }
}

// Usage
var session = new GameSessionBuilder()
    .WithGameName("F1 25")
    .Build();
```

### Fixtures
- Reusable test data
- Binary files for parsers
- In-memory database for repositories

```csharp
public static class TelemetryFixtures
{
    public static byte[] GetSampleGT7Packet()
    {
        return File.ReadAllBytes("Fixtures/TelemetryData/GT7SamplePacket.bin");
    }
    
    public static GameSession CreateValidSession()
    {
        return new GameSessionBuilder().Build();
    }
}
```

## Test Execution

### Commands
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run only unit tests
dotnet test --filter "FullyQualifiedName~Unit"

# Run only integration tests
dotnet test --filter "FullyQualifiedName~Integration"

# Run tests from a specific class
dotnet test --filter "FullyQualifiedName~CreateSessionHandlerTests"
```

### Filters
- Use `[Trait]` to categorize tests
- Filter by category in CLI

```csharp
[Fact]
[Trait("Category", "Unit")]
[Trait("Category", "Domain")]
public async Task Handle_ValidCommand_ShouldCreateSession()
{
    // Test
}
```

## Test Coverage

### Minimum Requirements
- All handlers must have tests
- All parsers must have tests with real/mock data
- Critical repositories must have integration tests
- Public endpoints must have tests

### Target Coverage
- **Minimum:** 80% on critical changes
- **Ideal:** 90%+ on business logic
- **Acceptable:** 70%+ on infrastructure code

## Anti-patterns to Avoid

- Tests that depend on each other
- Non-deterministic tests (depend on time, network, etc.)
- Tests that test multiple things at once
- Weak or ambiguous assertions
- Ignoring failing tests ("I'll fix it later")
- Tests without Assert phase
- Mocking everything (better to use real implementations when possible)
- Slow tests without reason (use in-memory when applicable)

# Racing Coach - Project Conventions

## Project Context

Racing Coach is an analysis and coaching platform for racing/sim racing games.

**Objective:** Create a system that receives telemetry data from racing games, normalizes it into a common model, analyzes sessions/laps/sectors, detects improvement areas, and generates personalized recommendations using AI/LLM.

**Target games:**
- Gran Turismo 7 (PS5) - initial
- F1 25 - future
- Other simulators - later expansion

**Technology stack:**
- .NET 10 + Blazor Server
- SQLite with Entity Framework Core
- xUnit + FluentAssertions + NSubstitute for testing
- Local deployment

**Architecture:**
- Clean Architecture with 5 layers: Api, Domain, Infrastructure, Shared, Providers
- Modular provider system (each game is a separate project)
- Minimal API for endpoints
- Command + Handler Pattern
- Repository Pattern
- Result/Error Pattern
- Phased dependency injection

## Language Conventions

**IMPORTANT: All code, comments, documentation, and AGENTS.md files must be written in English.**

- All C# code must be in English
- All comments (inline, XML docs) must be in English
- All AGENTS.md files must be in English
- All commit messages must be in English
- All variable names, method names, and class names must be in English
- All documentation (README, etc.) must be in English

## Code Conventions

### Clean Code
- Clean, readable, and maintainable code
- Descriptive and consistent naming
- Small methods with single responsibility
- No unnecessary comments (code should be self-explanatory)
- Strictly applied SOLID principles

### Design Patterns
- **Command Pattern:** Each business operation is a command with its handler
- **Repository Pattern:** Interfaces in Domain, implementations in Infrastructure
- **Result Pattern:** Use `Result<T>` for error handling (no exceptions for normal flow)
- **DTO Pattern:** Strongly typed DTOs for data transfer

### Strong Typing
- Use specific types instead of primitives when possible
- Nullable reference types enabled
- Avoid `dynamic` and `object` unless justified
- Records for immutable DTOs

### Layer Structure
- **Domain:** Domain models, repository interfaces, commands/handlers, domain services. No external dependencies.
- **Infrastructure:** Concrete implementations (EF Core, database configurations). Depends on Domain.
- **Api:** Minimal API endpoints, Blazor components, application services. Depends on Domain.
- **Shared:** Shared DTOs, constants, transversal helpers. No business logic.
- **Providers:** Game-specific implementations (parsers, listeners). Each provider is a separate project.

### Dependency Rule
- Domain does NOT depend on Api or Infrastructure
- Infrastructure depends on Domain
- Api depends on Domain
- Shared is transversal and can be referenced by all

## Commit Conventions

We use standard **Conventional Commits**:

```
<type>: <description>

[optional body]
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Format changes (spaces, commas, etc.) without changing logic
- `refactor`: Code refactoring
- `test`: Adding or modifying tests
- `chore`: Build, dependencies, configuration changes

**Examples:**
```
feat: add UDP listener for telemetry capture
fix: correct malformed UDP packet parsing
docs: update README with installation instructions
refactor: extract parsing logic to specific class
test: add tests for GT7TelemetryParser
```

## Build and Testing

### Commands
```bash
# Build entire solution
dotnet build

# Run tests
dotnet test

# Run application
dotnet run --project src/RacingCoach.Api

# Restore packages
dotnet restore
```

### Mandatory Testing
- **All new code must have tests**
- Unit tests for each handler/command
- Integration tests for critical endpoints
- Telemetry parser tests with real/mock data
- Minimum coverage for critical changes: 80%

### Before Commit
1. Build without errors: `dotnet build`
2. All tests pass: `dotnet test`
3. No critical warnings
4. Formatted code (if formatter is configured)

## Workflow

### Branches
- `main`: Stable and tested code
- `develop`: Continuous integration (optional)
- `feature/*`: New features
- `fix/*`: Bug fixes
- `refactor/*`: Refactoring

### Pull Requests
- All changes go through PR (except urgent hotfixes)
- PR must include clear description of changes
- Tests must pass before merge
- Code review mandatory (if team exists)

## Solution Structure

```
RacingCoach/
├── RacingCoach.sln
├── Directory.Build.props              # Common build config
├── Directory.Packages.props           # Central Package Management
├── src/
│   ├── RacingCoach.Api/               # Blazor Server + Endpoints
│   ├── RacingCoach.Domain/            # Models, interfaces, commands, services
│   ├── RacingCoach.Infrastructure/    # SQLite, EF Core, repositories
│   ├── RacingCoach.Shared/            # DTOs, constants, helpers
│   └── RacingCoach.Providers.GT7/     # GT7-specific parser and listener
└── tests/
    └── RacingCoach.Tests/             # xUnit tests
```

## Naming Conventions

### C#
- **Classes/Interfaces:** PascalCase (`TelemetryParser`, `ITelemetryRepository`)
- **Methods:** PascalCase (`ParsePacket`, `GetSessionById`)
- **Variables/Parameters:** camelCase (`rawPacket`, `sessionId`)
- **Private fields:** camelCase with optional `_` prefix (`_repository`, `_logger`)
- **Constants:** PascalCase (`DefaultUdpPort`, `MaxPacketSize`)

### Files and Folders
- **C# files:** PascalCase (`TelemetryParser.cs`, `GameSession.cs`)
- **Folders:** PascalCase (`Models/`, `Commands/`, `Repositories/`)
- **Projects:** PascalCase with prefix (`RacingCoach.Domain`)

## Dependency Injection

### Registration Order in Program.cs
```csharp
builder.Services.AddDomain();
builder.Services.AddInfrastructure(configuration);
builder.Services.AddApi(configuration);
builder.Services.AddGT7Provider();
```

### Extension Methods
Each layer has its own `Add{Layer}()` extension method:
- `AddDomain()` in Domain
- `AddInfrastructure(IConfiguration)` in Infrastructure
- `AddApi(IConfiguration)` in Api
- `AddGT7Provider()` in Providers.GT7

## Error Handling

### Result Pattern
```csharp
public Result<Session> GetSession(Guid sessionId)
{
    var session = _repository.GetById(sessionId);
    if (session is null)
        return Result.Failure<Session>(Error.NotFound("Session not found"));
    
    return Result.Success(session);
}
```

### Exceptions
- Only for truly exceptional cases
- Never for normal control flow
- Catch at boundaries (endpoints, handlers) and convert to Result

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=racingcoach.db"
  }
}
```

### Options Pattern
Use `IOptions<T>` for typed configuration:
```csharp
public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}
```

## Documentation

### XML Comments
- Only for complex public APIs
- Don't document the obvious
- Focus on "why", not "what"

### README
- Installation instructions
- How to run the application
- How to run tests
- Project structure

## Roadmap

### Phase 1: Packet Sniffer (Completed)
- [x] Base project structure
- [x] UDP Listener
- [x] Raw packet capture
- [x] Basic UI for visualization
- [x] GT7 packet format analysis

### Phase 2: Parser + Visualization (Completed)
- [x] GT7 parser
- [x] Normalized telemetry model
- [x] Real-time visualization
- [x] Basic session analysis

### Phase 3: Modular Architecture (Completed)
- [x] Provider abstraction layer
- [x] GT7 provider implementation
- [x] Session management system
- [x] Configuration persistence
- [x] REST API endpoints
- [x] UI for provider/session management

### Phase 4: Analysis + AI (Future)
- [ ] Lap/sector detection
- [ ] Performance metrics
- [ ] LLM integration for recommendations
- [ ] Driver profile

## Important Notes

- **Do not modify opencode configuration** - opencode automatically reads AGENTS.md
- **Mandatory testing** - Do not commit without passing tests
- **Strict Clean Code** - Readable code > "clever" code
- **Strong typing** - Avoid dynamic/object unless justified
- **Result pattern** - Do not use exceptions for normal flow
- **English only** - All code, comments, and documentation must be in English

# RacingCoach.Api - Conventions

## Responsibility

Presentation and communication layer. Contains:
- Blazor components (UI)
- Minimal API endpoints
- BackgroundServices (UDP Listener, etc.)
- Application configuration

## Dependencies

- Depends on **Domain** (interfaces, models, commands)
- Depends on **Shared** (DTOs, constants)
- Does **NOT** depend on Infrastructure directly (uses DI)

## Folder Structure

```
Api/
├── Components/           # Blazor components
│   ├── Pages/           # Application pages
│   ├── Layout/          # Layouts and shared components
│   └── Shared/          # Reusable components
├── Endpoints/           # Minimal API endpoints
│   └── FeatureX/
│       ├── FeatureXEndpoints.cs
│       ├── FeatureXEndpointsParameters.cs
│       └── FeatureXEndpointsResults.cs
├── Services/            # BackgroundServices and application services
├── Extensions/          # Extension methods for DI
└── wwwroot/             # Static files
```

## Specific Conventions

### Minimal API
- Group endpoints by feature using `MapGroup`
- Thin endpoints: only orchestrate, don't implement business logic
- Use `*ApiParameter` and `*ApiResult` classes for contracts
- Early validations before invoking handlers

```csharp
public static class SessionEndpoints
{
    public static void MapSessionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sessions");
        
        group.MapGet("/", GetAllSessions);
        group.MapGet("/{id:guid}", GetSessionById);
    }
    
    private static async Task<IResult> GetAllSessions(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllSessionsQuery(), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error.ToString());
    }
}
```

### BackgroundServices
- Inherit from `BackgroundService` for background tasks
- Inject dependencies via constructor
- Handle cancellation tokens correctly
- Structured logging of important events

```csharp
public class UdpListenerService : BackgroundService
{
    private readonly ILogger<UdpListenerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly UdpListenerOptions _options;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Implementation
    }
}
```

### Blazor Components
- Use `@code` block at the end of the file
- Inject services with `@inject`
- Handle loading and error states explicitly
- Separate business logic into services, not components

### Configuration
- Use `IOptions<T>` for typed configuration
- Validate configuration in service registration
- Don't hardcode magic values

```csharp
public class UdpListenerOptions
{
    public const string SectionName = "UdpListener";
    
    public int Port { get; set; } = 3333;
    public bool Enabled { get; set; } = true;
}

// In Program.cs
builder.Services.Configure<UdpListenerOptions>(
    builder.Configuration.GetSection(UdpListenerOptions.SectionName));
```

### Dependency Injection
- Extension method `AddApi(this IServiceCollection, IConfiguration)` to register this layer's services
- Register BackgroundServices with `AddHostedService<T>()`
- Register Blazor components if necessary

```csharp
public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<UdpListenerOptions>(
            configuration.GetSection(UdpListenerOptions.SectionName));
        
        services.AddHostedService<UdpListenerService>();
        
        return services;
    }
}
```

## Anti-patterns to Avoid

- Business logic in Blazor components
- Endpoints that directly access databases
- BackgroundServices that don't handle cancellation tokens
- Hardcoding configuration in code
- Mixing responsibilities from different layers

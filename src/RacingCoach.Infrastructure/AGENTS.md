# RacingCoach.Infrastructure - Conventions

## Responsibility

Technical infrastructure layer. Contains:
- Concrete repository implementations
- Entity Framework Core (DbContext, configurations)
- Game-specific telemetry parsers
- External service integrations
- Options configuration

## Dependencies

- Depends on **Domain** (implements interfaces)
- Depends on **Shared** (DTOs, constants)
- Is **NOT** referenced by Domain

## Folder Structure

```
Infrastructure/
├── Persistence/         # EF Core and repositories
│   ├── ApplicationDbContext.cs
│   ├── Configurations/  # Fluent API configurations
│   │   └── RawTelemetryPacketConfiguration.cs
│   └── Repositories/
│       ├── RawPacketRepository.cs
│       └── SessionRepository.cs
├── Parsers/             # Game-specific telemetry parsers
│   ├── ITelemetryParser.cs
│   ├── GT7TelemetryParser.cs
│   └── F1TelemetryParser.cs (future)
├── Options/             # Options classes
│   └── UdpListenerOptions.cs
└── Extensions/          # Extension methods for DI
```

## Specific Conventions

### Entity Framework Core
- Use Fluent API for configurations (no Data Annotations on entities)
- Separate configurations into individual files
- DbContext in Infrastructure, not in Domain
- Controlled and versioned migrations

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<RawTelemetryPacket> RawTelemetryPackets { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

### Fluent API Configurations
- One configuration per entity
- Name files as `{Entity}Configuration.cs`
- Configure all properties explicitly

```csharp
public class RawTelemetryPacketConfiguration : IEntityTypeConfiguration<RawTelemetryPacket>
{
    public void Configure(EntityTypeBuilder<RawTelemetryPacket> builder)
    {
        builder.ToTable("RawTelemetryPackets");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Timestamp)
            .IsRequired();
        
        builder.Property(p => p.Data)
            .IsRequired();
        
        builder.HasIndex(p => p.SessionId);
        builder.HasIndex(p => p.Timestamp);
    }
}
```

### Repositories
- Implement Domain interfaces
- Use EF Core for persistence
- Map between domain entities and persistence models if necessary
- Return `Result<T>` consistently

```csharp
public class RawPacketRepository : IRawPacketRepository
{
    private readonly ApplicationDbContext _context;
    
    public RawPacketRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<RawTelemetryPacket>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var packet = await _context.RawTelemetryPackets
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        
        return packet is null
            ? Result<RawTelemetryPacket>.Failure(Error.NotFound($"Packet {id} not found"))
            : Result<RawTelemetryPacket>.Success(packet);
    }
    
    public async Task<Result<Guid>> AddAsync(RawTelemetryPacket packet, CancellationToken cancellationToken = default)
    {
        _context.RawTelemetryPackets.Add(packet);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<Guid>.Success(packet.Id);
    }
}
```

### Telemetry Parsers
- Common `ITelemetryParser` interface in Domain
- Game-specific implementations in Infrastructure
- Each parser knows its game's binary format
- Return normalized model or raw bytes

```csharp
// In Domain
public interface ITelemetryParser
{
    string GameName { get; }
    Result<NormalizedTelemetry> Parse(byte[] data);
}

// In Infrastructure
public class GT7TelemetryParser : ITelemetryParser
{
    public string GameName => "Gran Turismo 7";
    
    public Result<NormalizedTelemetry> Parse(byte[] data)
    {
        // Parse GT7-specific format
        // Return normalized model
    }
}
```

### Options Pattern
- Use `IOptions<T>` for configuration
- Validate options in registration
- Options classes in Infrastructure or Shared

```csharp
public class UdpListenerOptions
{
    public const string SectionName = "UdpListener";
    
    public int Port { get; set; } = 3333;
    public bool Enabled { get; set; } = true;
    public int BufferSize { get; set; } = 4096;
}
```

## Dependency Injection

Extension method `AddInfrastructure(IConfiguration)` to register services:

```csharp
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });
        
        // Register repositories
        services.AddScoped<IRawPacketRepository, RawPacketRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        
        // Register parsers
        services.AddScoped<ITelemetryParser, GT7TelemetryParser>();
        
        // Configure options
        services.Configure<UdpListenerOptions>(
            configuration.GetSection(UdpListenerOptions.SectionName));
        
        return services;
    }
}
```

## Database

### SQLite
- Local file: `racingcoach.db`
- Connection string in `appsettings.json`
- Automatic migrations in development (optional)
- Periodic backup recommended

### Migrations
```bash
# Create migration
dotnet ef migrations add InitialCreate --project src/RacingCoach.Infrastructure

# Apply migrations
dotnet ef database update --project src/RacingCoach.Infrastructure
```

## Anti-patterns to Avoid

- Business logic in repositories
- Dependencies from Domain to Infrastructure
- Complex queries in repositories (use specifications if they grow)
- Hardcoding connection strings or configurations
- Mixing parsing and persistence responsibilities
- Ignoring EF Core errors (always handle exceptions)

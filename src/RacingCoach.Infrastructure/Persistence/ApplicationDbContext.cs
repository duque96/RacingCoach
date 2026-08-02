using Microsoft.EntityFrameworkCore;
using RacingCoach.Domain.Models.Sessions;
using RacingCoach.Infrastructure.Persistence.Entities;

namespace RacingCoach.Infrastructure.Persistence;

internal class ApplicationDbContext : DbContext
{
    public DbSet<DbTelemetryData> TelemetryData { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<DbProviderConfiguration> ProviderConfigurations { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RacingCoach.Domain.Interfaces;
using RacingCoach.Infrastructure.Persistence;
using RacingCoach.Infrastructure.Persistence.Repositories;

namespace RacingCoach.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITelemetryDataRepository, TelemetryDataRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IProviderConfigurationRepository, ProviderConfigurationRepository>();

        return services;
    }

    public static void MigrateDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
}

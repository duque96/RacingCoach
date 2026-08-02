using Microsoft.Extensions.DependencyInjection;
using RacingCoach.Domain.Services;

namespace RacingCoach.Domain.Extensions;

public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainServiceCollectionExtensions).Assembly));

        services.AddSingleton<TelemetrySessionManager>();

        return services;
    }
}

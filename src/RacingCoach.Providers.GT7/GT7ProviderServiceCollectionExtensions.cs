using Microsoft.Extensions.DependencyInjection;
using RacingCoach.Domain.Interfaces;

namespace RacingCoach.Providers.GT7;

public static class GT7ProviderServiceCollectionExtensions
{
    public static IServiceCollection AddGT7Provider(this IServiceCollection services)
    {
        services.AddSingleton<ITelemetryProvider, GT7TelemetryProvider>();

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using RacingCoach.Api.Services;

namespace RacingCoach.Api.Extensions;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<TelemetryService>();
        services.AddScoped<TelemetryHistoryService>();

        return services;
    }
}

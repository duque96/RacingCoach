using RacingCoach.Domain.Interfaces;
using RacingCoach.Domain.Models.Providers;
using RacingCoach.Domain.Services;

namespace RacingCoach.Api.Endpoints;

public static class SessionEndpoints
{
    public static void MapSessionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sessions");

        group.MapGet("/active", GetActiveSession);
        group.MapPost("/start", StartSession);
        group.MapPost("/stop", StopSession);
    }

    private static IResult GetActiveSession(TelemetrySessionManager sessionManager)
    {
        var session = sessionManager.ActiveSession;
        return session is null ? Results.Ok(new { Active = false }) : Results.Ok(new { Active = true, Session = session });
    }

    private static async Task<IResult> StartSession(
        StartSessionRequest request,
        TelemetrySessionManager sessionManager,
        IProviderConfigurationRepository configRepository)
    {
        var configResult = await configRepository.GetByIdAsync(request.ConfigurationId);
        if (!configResult.IsSuccess)
            return Results.BadRequest(new { Error = "Configuration not found" });

        var result = await sessionManager.StartSessionAsync(request.ProviderId, request.ConfigurationId);
        if (!result.IsSuccess)
            return Results.BadRequest(new { Error = result.Error.Message });

        return Results.Ok(new { SessionId = result.Value.Id, GameName = result.Value.GameName });
    }

    private static async Task<IResult> StopSession(TelemetrySessionManager sessionManager)
    {
        var result = await sessionManager.StopSessionAsync();
        if (!result.IsSuccess)
            return Results.BadRequest(new { Error = result.Error.Message });

        return Results.Ok(new { Message = "Session stopped" });
    }
}

public record StartSessionRequest(string ProviderId, Guid ConfigurationId);

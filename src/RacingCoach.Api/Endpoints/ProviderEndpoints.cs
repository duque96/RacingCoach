using RacingCoach.Domain.Interfaces;
using RacingCoach.Domain.Models.Providers;

namespace RacingCoach.Api.Endpoints;

public static class ProviderEndpoints
{
    public static void MapProviderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/providers");

        group.MapGet("/", GetProviders);
        group.MapGet("/configurations", GetConfigurations);
        group.MapGet("/configurations/{providerId}", GetConfigurationsByProvider);
        group.MapPost("/configurations", CreateConfiguration);
        group.MapPut("/configurations/{id}", UpdateConfiguration);
        group.MapDelete("/configurations/{id}", DeleteConfiguration);
    }

    private static IResult GetProviders(IEnumerable<ITelemetryProvider> providers)
    {
        var result = providers.Select(p => new
        {
            p.Id,
            p.Name,
            p.GameName,
            Schema = p.ConfigurationSchema.Fields.Select(f => new
            {
                f.Name,
                f.DisplayName,
                f.Type,
                f.Required,
                f.DefaultValue,
                f.Description
            })
        });
        return Results.Ok(result);
    }

    private static async Task<IResult> GetConfigurations(IProviderConfigurationRepository repository)
    {
        var result = await repository.GetAllAsync();
        if (!result.IsSuccess)
            return Results.Problem(result.Error.Message);

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetConfigurationsByProvider(string providerId, IProviderConfigurationRepository repository)
    {
        var result = await repository.GetByProviderIdAsync(providerId);
        if (!result.IsSuccess)
            return Results.Problem(result.Error.Message);

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> CreateConfiguration(
        CreateConfigurationRequest request,
        IProviderConfigurationRepository repository)
    {
        var config = new ProviderConfiguration(request.ProviderId, request.Name, request.Settings);
        var result = await repository.AddAsync(config);
        if (!result.IsSuccess)
            return Results.Problem(result.Error.Message);

        return Results.Created($"/api/providers/configurations/{result.Value}", new { Id = result.Value });
    }

    private static async Task<IResult> UpdateConfiguration(
        Guid id,
        UpdateConfigurationRequest request,
        IProviderConfigurationRepository repository)
    {
        var getResult = await repository.GetByIdAsync(id);
        if (!getResult.IsSuccess)
            return Results.NotFound();

        var config = getResult.Value;
        config.Update(request.Name, request.Settings);

        var result = await repository.UpdateAsync(config);
        if (!result.IsSuccess)
            return Results.Problem(result.Error.Message);

        return Results.Ok(new { Message = "Configuration updated" });
    }

    private static async Task<IResult> DeleteConfiguration(Guid id, IProviderConfigurationRepository repository)
    {
        var result = await repository.DeleteAsync(id);
        if (!result.IsSuccess)
            return Results.NotFound();

        return Results.Ok(new { Message = "Configuration deleted" });
    }
}

public record CreateConfigurationRequest(string ProviderId, string Name, Dictionary<string, string> Settings);
public record UpdateConfigurationRequest(string Name, Dictionary<string, string> Settings);

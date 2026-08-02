using Microsoft.EntityFrameworkCore;
using RacingCoach.Domain.Common;
using RacingCoach.Domain.Interfaces;
using RacingCoach.Domain.Models.Providers;
using RacingCoach.Infrastructure.Persistence.Entities;
using System.Text.Json;

namespace RacingCoach.Infrastructure.Persistence.Repositories;

internal class ProviderConfigurationRepository : IProviderConfigurationRepository
{
    private readonly ApplicationDbContext _context;

    public ProviderConfigurationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProviderConfiguration>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProviderConfigurations
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result<ProviderConfiguration>.Failure(Error.NotFound($"Provider configuration {id} not found"));

        return Result<ProviderConfiguration>.Success(MapToDomain(entity));
    }

    public async Task<Result<IEnumerable<ProviderConfiguration>>> GetByProviderIdAsync(string providerId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.ProviderConfigurations
            .Where(e => e.ProviderId == providerId)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<ProviderConfiguration>>.Success(entities.Select(MapToDomain));
    }

    public async Task<Result<IEnumerable<ProviderConfiguration>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.ProviderConfigurations
            .OrderBy(e => e.ProviderId)
            .ThenBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<ProviderConfiguration>>.Success(entities.Select(MapToDomain));
    }

    public async Task<Result<Guid>> AddAsync(ProviderConfiguration config, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(config);
        _context.ProviderConfigurations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result> UpdateAsync(ProviderConfiguration config, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProviderConfigurations
            .FirstOrDefaultAsync(e => e.Id == config.Id, cancellationToken);

        if (entity is null)
            return Result.Failure(Error.NotFound($"Provider configuration {config.Id} not found"));

        entity.Name = config.Name;
        entity.SettingsJson = JsonSerializer.Serialize(config.Settings);
        entity.UpdatedAt = config.UpdatedAt;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProviderConfigurations
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result.Failure(Error.NotFound($"Provider configuration {id} not found"));

        _context.ProviderConfigurations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static ProviderConfiguration MapToDomain(DbProviderConfiguration entity)
    {
        var settings = string.IsNullOrEmpty(entity.SettingsJson)
            ? new Dictionary<string, string>()
            : JsonSerializer.Deserialize<Dictionary<string, string>>(entity.SettingsJson) ?? new Dictionary<string, string>();

        var config = new ProviderConfiguration(entity.ProviderId, entity.Name, settings);

        var idField = typeof(ProviderConfiguration).GetField("<Id>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        idField?.SetValue(config, entity.Id);

        var createdAtField = typeof(ProviderConfiguration).GetField("<CreatedAt>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        createdAtField?.SetValue(config, entity.CreatedAt);

        var updatedAtField = typeof(ProviderConfiguration).GetField("<UpdatedAt>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        updatedAtField?.SetValue(config, entity.UpdatedAt);

        return config;
    }

    private static DbProviderConfiguration MapToEntity(ProviderConfiguration config)
    {
        return new DbProviderConfiguration
        {
            Id = config.Id,
            ProviderId = config.ProviderId,
            Name = config.Name,
            SettingsJson = JsonSerializer.Serialize(config.Settings),
            CreatedAt = config.CreatedAt,
            UpdatedAt = config.UpdatedAt
        };
    }
}

using Microsoft.EntityFrameworkCore;
using RacingCoach.Domain.Common;
using RacingCoach.Domain.Interfaces;
using RacingCoach.Domain.Models.Telemetry;
using RacingCoach.Infrastructure.Persistence;
using RacingCoach.Infrastructure.Persistence.Entities;

namespace RacingCoach.Infrastructure.Persistence.Repositories;

internal class TelemetryDataRepository : ITelemetryDataRepository
{
    private readonly ApplicationDbContext _context;

    public TelemetryDataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TelemetryData>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbTelemetry = await _context.TelemetryData
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (dbTelemetry is null)
            return Result<TelemetryData>.Failure(Error.NotFound($"TelemetryData {id} not found"));

        return Result<TelemetryData>.Success(MapToDomain(dbTelemetry));
    }

    public async Task<Result<IEnumerable<TelemetryData>>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var dbTelemetryList = await _context.TelemetryData
            .Where(t => t.SessionId == sessionId)
            .OrderBy(t => t.Timestamp)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<TelemetryData>>.Success(dbTelemetryList.Select(MapToDomain));
    }

    public async Task<Result<IEnumerable<TelemetryData>>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
    {
        var dbTelemetryList = await _context.TelemetryData
            .OrderByDescending(t => t.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<TelemetryData>>.Success(dbTelemetryList.Select(MapToDomain));
    }

    public async Task<Result<Guid>> AddAsync(TelemetryData telemetryData, CancellationToken cancellationToken = default)
    {
        var dbTelemetry = MapToDb(telemetryData);
        _context.TelemetryData.Add(dbTelemetry);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(dbTelemetry.Id);
    }

    public async Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var count = await _context.TelemetryData.CountAsync(cancellationToken);
        return Result<int>.Success(count);
    }

    public async Task<Result<int>> GetCountBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var count = await _context.TelemetryData
            .CountAsync(t => t.SessionId == sessionId, cancellationToken);

        return Result<int>.Success(count);
    }

    private static TelemetryData MapToDomain(DbTelemetryData db)
    {
        return new TelemetryData(
            sessionId: db.SessionId,
            timestamp: db.Timestamp,
            speed: db.Speed,
            rpm: db.RPM,
            gear: db.Gear,
            throttle: db.Throttle,
            brake: db.Brake,
            steering: db.Steering,
            positionX: db.PositionX,
            positionY: db.PositionY,
            positionZ: db.PositionZ,
            velocityX: db.VelocityX,
            velocityY: db.VelocityY,
            velocityZ: db.VelocityZ,
            accelerationX: db.AccelerationX,
            accelerationY: db.AccelerationY,
            accelerationZ: db.AccelerationZ,
            tireTempFL: db.TireTempFL,
            tireTempFR: db.TireTempFR,
            tireTempRL: db.TireTempRL,
            tireTempRR: db.TireTempRR,
            brakeTempFL: db.BrakeTempFL,
            brakeTempFR: db.BrakeTempFR,
            brakeTempRL: db.BrakeTempRL,
            brakeTempRR: db.BrakeTempRR,
            suspensionFL: db.SuspensionFL,
            suspensionFR: db.SuspensionFR,
            suspensionRL: db.SuspensionRL,
            suspensionRR: db.SuspensionRR,
            fuelLevel: db.FuelLevel,
            fuelCapacity: db.FuelCapacity,
            currentLap: db.CurrentLap,
            totalLaps: db.TotalLaps,
            sector: db.Sector
        );
    }

    private static DbTelemetryData MapToDb(TelemetryData domain)
    {
        return new DbTelemetryData
        {
            Id = domain.Id,
            SessionId = domain.SessionId,
            Timestamp = domain.Timestamp,
            Speed = domain.Speed,
            RPM = domain.RPM,
            Gear = domain.Gear,
            Throttle = domain.Throttle,
            Brake = domain.Brake,
            Steering = domain.Steering,
            PositionX = domain.PositionX,
            PositionY = domain.PositionY,
            PositionZ = domain.PositionZ,
            VelocityX = domain.VelocityX,
            VelocityY = domain.VelocityY,
            VelocityZ = domain.VelocityZ,
            AccelerationX = domain.AccelerationX,
            AccelerationY = domain.AccelerationY,
            AccelerationZ = domain.AccelerationZ,
            TireTempFL = domain.TireTempFL,
            TireTempFR = domain.TireTempFR,
            TireTempRL = domain.TireTempRL,
            TireTempRR = domain.TireTempRR,
            BrakeTempFL = domain.BrakeTempFL,
            BrakeTempFR = domain.BrakeTempFR,
            BrakeTempRL = domain.BrakeTempRL,
            BrakeTempRR = domain.BrakeTempRR,
            SuspensionFL = domain.SuspensionFL,
            SuspensionFR = domain.SuspensionFR,
            SuspensionRL = domain.SuspensionRL,
            SuspensionRR = domain.SuspensionRR,
            FuelLevel = domain.FuelLevel,
            FuelCapacity = domain.FuelCapacity,
            CurrentLap = domain.CurrentLap,
            TotalLaps = domain.TotalLaps,
            Sector = domain.Sector
        };
    }
}

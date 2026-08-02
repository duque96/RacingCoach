using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RacingCoach.Infrastructure.Persistence.Entities;

namespace RacingCoach.Infrastructure.Persistence.Configurations;

internal class DbTelemetryDataConfiguration : IEntityTypeConfiguration<DbTelemetryData>
{
    public void Configure(EntityTypeBuilder<DbTelemetryData> builder)
    {
        builder.ToTable("TelemetryData");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.SessionId)
            .IsRequired();

        builder.Property(t => t.Timestamp)
            .IsRequired();

        builder.Property(t => t.Speed)
            .IsRequired();

        builder.Property(t => t.RPM)
            .IsRequired();

        builder.Property(t => t.Gear)
            .IsRequired();

        builder.Property(t => t.Throttle)
            .IsRequired();

        builder.Property(t => t.Brake)
            .IsRequired();

        builder.Property(t => t.Steering)
            .IsRequired();

        builder.Property(t => t.PositionX)
            .IsRequired();

        builder.Property(t => t.PositionY)
            .IsRequired();

        builder.Property(t => t.PositionZ)
            .IsRequired();

        builder.Property(t => t.VelocityX)
            .IsRequired();

        builder.Property(t => t.VelocityY)
            .IsRequired();

        builder.Property(t => t.VelocityZ)
            .IsRequired();

        builder.Property(t => t.AccelerationX)
            .IsRequired();

        builder.Property(t => t.AccelerationY)
            .IsRequired();

        builder.Property(t => t.AccelerationZ)
            .IsRequired();

        builder.Property(t => t.TireTempFL)
            .IsRequired();

        builder.Property(t => t.TireTempFR)
            .IsRequired();

        builder.Property(t => t.TireTempRL)
            .IsRequired();

        builder.Property(t => t.TireTempRR)
            .IsRequired();

        builder.Property(t => t.BrakeTempFL)
            .IsRequired();

        builder.Property(t => t.BrakeTempFR)
            .IsRequired();

        builder.Property(t => t.BrakeTempRL)
            .IsRequired();

        builder.Property(t => t.BrakeTempRR)
            .IsRequired();

        builder.Property(t => t.SuspensionFL)
            .IsRequired();

        builder.Property(t => t.SuspensionFR)
            .IsRequired();

        builder.Property(t => t.SuspensionRL)
            .IsRequired();

        builder.Property(t => t.SuspensionRR)
            .IsRequired();

        builder.Property(t => t.FuelLevel)
            .IsRequired();

        builder.Property(t => t.FuelCapacity)
            .IsRequired();

        builder.Property(t => t.CurrentLap)
            .IsRequired();

        builder.Property(t => t.TotalLaps)
            .IsRequired();

        builder.Property(t => t.Sector)
            .IsRequired();

        builder.HasIndex(t => t.SessionId);
        builder.HasIndex(t => t.Timestamp);
    }
}

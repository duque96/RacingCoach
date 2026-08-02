using System.Text;
using RacingCoach.Domain.Common;
using RacingCoach.Domain.Interfaces;
using RacingCoach.Domain.Models.Telemetry;

namespace RacingCoach.Providers.GT7;

internal class GT7TelemetryParser : ITelemetryParser
{
    private const string MagicKey = "Simulator Interface Packet GT7 ver 0.0";
    private const uint MagicNumber = 0x47375330;

    public string GameName => "Gran Turismo 7";

    public Result<TelemetryData> Parse(byte[] data)
    {
        try
        {
            if (data.Length < 0x44)
                return Result<TelemetryData>.Failure(Error.Validation("Packet too short"));

            byte[] ivBytes = new byte[8];
            uint iv1 = BitConverter.ToUInt32(data, 0x40);
            uint iv2 = iv1 ^ 0xDEADBEAF;
            
            BitConverter.GetBytes(iv2).CopyTo(ivBytes, 0);
            BitConverter.GetBytes(iv1).CopyTo(ivBytes, 4);

            byte[] key = Encoding.ASCII.GetBytes(MagicKey);
            if (key.Length > 32)
            {
                byte[] truncatedKey = new byte[32];
                Array.Copy(key, truncatedKey, 32);
                key = truncatedKey;
            }
            Salsa20 salsa20 = new Salsa20(key, ivBytes);

            byte[] decrypted = salsa20.Decrypt(data);

            if (decrypted.Length < 4)
                return Result<TelemetryData>.Failure(Error.Validation("Decrypted packet too short"));

            uint magic = BitConverter.ToUInt32(decrypted, 0);
            if (magic != MagicNumber)
                return Result<TelemetryData>.Failure(Error.Validation($"Invalid magic number: 0x{magic:X8}"));

            GT7TelemetryData gt7Data = ParseGT7Data(decrypted);

            TelemetryData telemetryData = ConvertToTelemetryData(gt7Data);

            return Result<TelemetryData>.Success(telemetryData);
        }
        catch (Exception ex)
        {
            return Result<TelemetryData>.Failure(Error.Internal($"Failed to parse GT7 telemetry: {ex.Message}"));
        }
    }

    private GT7TelemetryData ParseGT7Data(byte[] data)
    {
        GT7TelemetryData telemetry = new GT7TelemetryData
        {
            PositionX = BitConverter.ToSingle(data, 0x04),
            PositionY = BitConverter.ToSingle(data, 0x08),
            PositionZ = BitConverter.ToSingle(data, 0x0C),

            VelocityX = BitConverter.ToSingle(data, 0x10),
            VelocityY = BitConverter.ToSingle(data, 0x14),
            VelocityZ = BitConverter.ToSingle(data, 0x18),

            RotationPitch = BitConverter.ToSingle(data, 0x1C),
            RotationYaw = BitConverter.ToSingle(data, 0x20),
            RotationRoll = BitConverter.ToSingle(data, 0x24),

            AngularVelocityX = BitConverter.ToSingle(data, 0x2C),
            AngularVelocityY = BitConverter.ToSingle(data, 0x30),
            AngularVelocityZ = BitConverter.ToSingle(data, 0x34),

            RideHeight = BitConverter.ToSingle(data, 0x38) * 1000,

            RPM = BitConverter.ToSingle(data, 0x3C),

            WaterTemp = BitConverter.ToSingle(data, 0x58),
            OilTemp = BitConverter.ToSingle(data, 0x5C),
            OilPressure = BitConverter.ToSingle(data, 0x54),

            CurrentFuel = BitConverter.ToSingle(data, 0x44),
            FuelCapacity = BitConverter.ToSingle(data, 0x48),

            CarSpeed = BitConverter.ToSingle(data, 0x4C) * 3.6f,

            EstimatedTopSpeed = BitConverter.ToInt16(data, 0x8C),

            Boost = BitConverter.ToSingle(data, 0x50) - 1,

            TireTempFL = BitConverter.ToSingle(data, 0x60),
            TireTempFR = BitConverter.ToSingle(data, 0x64),
            TireTempRL = BitConverter.ToSingle(data, 0x68),
            TireTempRR = BitConverter.ToSingle(data, 0x6C),

            TireDiameterFL = BitConverter.ToSingle(data, 0xB4),
            TireDiameterFR = BitConverter.ToSingle(data, 0xB8),
            TireDiameterRL = BitConverter.ToSingle(data, 0xBC),
            TireDiameterRR = BitConverter.ToSingle(data, 0xC0),

            SuspensionFL = BitConverter.ToSingle(data, 0xC4),
            SuspensionFR = BitConverter.ToSingle(data, 0xC8),
            SuspensionRL = BitConverter.ToSingle(data, 0xCC),
            SuspensionRR = BitConverter.ToSingle(data, 0xD0),

            Clutch = BitConverter.ToSingle(data, 0xF4),
            ClutchEngaged = BitConverter.ToSingle(data, 0xF8),
            RPMAfterClutch = BitConverter.ToSingle(data, 0xFC),

            Gear1 = BitConverter.ToSingle(data, 0x104),
            Gear2 = BitConverter.ToSingle(data, 0x108),
            Gear3 = BitConverter.ToSingle(data, 0x10C),
            Gear4 = BitConverter.ToSingle(data, 0x110),
            Gear5 = BitConverter.ToSingle(data, 0x114),
            Gear6 = BitConverter.ToSingle(data, 0x118),
            Gear7 = BitConverter.ToSingle(data, 0x11C),
            Gear8 = BitConverter.ToSingle(data, 0x120),

            PackageId = BitConverter.ToInt32(data, 0x70),
            BestLap = BitConverter.ToInt32(data, 0x78),
            LastLap = BitConverter.ToInt32(data, 0x7C),
            CurrentLap = BitConverter.ToInt16(data, 0x74),
            TotalLaps = BitConverter.ToInt16(data, 0x76),

            CurrentPosition = BitConverter.ToInt16(data, 0x84),
            TotalPositions = BitConverter.ToInt16(data, 0x86),

            TimeOnTrack = BitConverter.ToInt32(data, 0x80) / 1000,

            CarId = BitConverter.ToInt32(data, 0x124),

            CurrentGear = (byte)(data[0x90] & 0x0F),
            SuggestedGear = (byte)(data[0x90] >> 4),

            Throttle = data[0x91] / 2.55f,
            Brake = data[0x92] / 2.55f,

            RPMRevWarning = BitConverter.ToUInt16(data, 0x88),
            RPMRevLimiter = BitConverter.ToUInt16(data, 0x8A),

            Timestamp = DateTime.UtcNow
        };

        byte flags1 = data[0x8E];
        byte flags2 = data[0x8F];

        telemetry.InRace = (flags1 & 0x01) != 0;
        telemetry.IsPaused = (flags1 & 0x02) != 0;
        telemetry.IsLoading = (flags1 & 0x04) != 0;
        telemetry.IsInGear = (flags1 & 0x08) != 0;
        telemetry.CarHasTurbo = (flags1 & 0x10) != 0;
        telemetry.IsRevLimiterFlashing = (flags1 & 0x20) != 0;
        telemetry.IsHandbrakeEngaged = (flags1 & 0x40) != 0;
        telemetry.IsLightsOn = (flags1 & 0x80) != 0;

        telemetry.IsLowBeamOn = (flags2 & 0x01) != 0;
        telemetry.IsHighBeamOn = (flags2 & 0x02) != 0;
        telemetry.IsASMEngaged = (flags2 & 0x04) != 0;
        telemetry.IsTCSEngaged = (flags2 & 0x08) != 0;

        telemetry.TireSpeedFL = Math.Abs(3.6f * BitConverter.ToSingle(data, 0xA4) * telemetry.TireDiameterFL);
        telemetry.TireSpeedFR = Math.Abs(3.6f * BitConverter.ToSingle(data, 0xA8) * telemetry.TireDiameterFR);
        telemetry.TireSpeedRL = Math.Abs(3.6f * BitConverter.ToSingle(data, 0xAC) * telemetry.TireDiameterRL);
        telemetry.TireSpeedRR = Math.Abs(3.6f * BitConverter.ToSingle(data, 0xB0) * telemetry.TireDiameterRR);

        if (telemetry.CarSpeed > 0)
        {
            telemetry.TireSlipRatioFL = telemetry.TireSpeedFL / telemetry.CarSpeed;
            telemetry.TireSlipRatioFR = telemetry.TireSpeedFR / telemetry.CarSpeed;
            telemetry.TireSlipRatioRL = telemetry.TireSpeedRL / telemetry.CarSpeed;
            telemetry.TireSlipRatioRR = telemetry.TireSpeedRR / telemetry.CarSpeed;
        }

        return telemetry;
    }

    private TelemetryData ConvertToTelemetryData(GT7TelemetryData gt7Data)
    {
        return new TelemetryData(
            sessionId: Guid.Empty,
            timestamp: gt7Data.Timestamp,
            speed: gt7Data.CarSpeed,
            rpm: gt7Data.RPM,
            gear: gt7Data.CurrentGear,
            throttle: gt7Data.Throttle,
            brake: gt7Data.Brake,
            steering: 0,
            positionX: gt7Data.PositionX,
            positionY: gt7Data.PositionY,
            positionZ: gt7Data.PositionZ,
            velocityX: gt7Data.VelocityX,
            velocityY: gt7Data.VelocityY,
            velocityZ: gt7Data.VelocityZ,
            accelerationX: 0,
            accelerationY: 0,
            accelerationZ: 0,
            tireTempFL: gt7Data.TireTempFL,
            tireTempFR: gt7Data.TireTempFR,
            tireTempRL: gt7Data.TireTempRL,
            tireTempRR: gt7Data.TireTempRR,
            brakeTempFL: 0,
            brakeTempFR: 0,
            brakeTempRL: 0,
            brakeTempRR: 0,
            suspensionFL: gt7Data.SuspensionFL,
            suspensionFR: gt7Data.SuspensionFR,
            suspensionRL: gt7Data.SuspensionRL,
            suspensionRR: gt7Data.SuspensionRR,
            fuelLevel: gt7Data.CurrentFuel,
            fuelCapacity: gt7Data.FuelCapacity,
            currentLap: gt7Data.CurrentLap,
            totalLaps: gt7Data.TotalLaps,
            sector: 0
        );
    }
}

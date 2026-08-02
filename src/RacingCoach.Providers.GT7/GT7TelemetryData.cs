namespace RacingCoach.Providers.GT7;

internal class GT7TelemetryData
{
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public float VelocityZ { get; set; }

    public float RotationPitch { get; set; }
    public float RotationYaw { get; set; }
    public float RotationRoll { get; set; }

    public float AngularVelocityX { get; set; }
    public float AngularVelocityY { get; set; }
    public float AngularVelocityZ { get; set; }

    public float RideHeight { get; set; }
    public float RPM { get; set; }

    public float WaterTemp { get; set; }
    public float OilTemp { get; set; }
    public float OilPressure { get; set; }

    public float CurrentFuel { get; set; }
    public float FuelCapacity { get; set; }

    public float CarSpeed { get; set; }
    public short EstimatedTopSpeed { get; set; }

    public float Boost { get; set; }

    public float TireTempFL { get; set; }
    public float TireTempFR { get; set; }
    public float TireTempRL { get; set; }
    public float TireTempRR { get; set; }

    public float TireDiameterFL { get; set; }
    public float TireDiameterFR { get; set; }
    public float TireDiameterRL { get; set; }
    public float TireDiameterRR { get; set; }

    public float TireSpeedFL { get; set; }
    public float TireSpeedFR { get; set; }
    public float TireSpeedRL { get; set; }
    public float TireSpeedRR { get; set; }

    public float TireSlipRatioFL { get; set; }
    public float TireSlipRatioFR { get; set; }
    public float TireSlipRatioRL { get; set; }
    public float TireSlipRatioRR { get; set; }

    public float SuspensionFL { get; set; }
    public float SuspensionFR { get; set; }
    public float SuspensionRL { get; set; }
    public float SuspensionRR { get; set; }

    public float Clutch { get; set; }
    public float ClutchEngaged { get; set; }
    public float RPMAfterClutch { get; set; }

    public float Gear1 { get; set; }
    public float Gear2 { get; set; }
    public float Gear3 { get; set; }
    public float Gear4 { get; set; }
    public float Gear5 { get; set; }
    public float Gear6 { get; set; }
    public float Gear7 { get; set; }
    public float Gear8 { get; set; }

    public int PackageId { get; set; }
    public int BestLap { get; set; }
    public int LastLap { get; set; }
    public short CurrentLap { get; set; }
    public short TotalLaps { get; set; }

    public short CurrentPosition { get; set; }
    public short TotalPositions { get; set; }

    public int TimeOnTrack { get; set; }

    public int CarId { get; set; }

    public byte CurrentGear { get; set; }
    public byte SuggestedGear { get; set; }
    public float Throttle { get; set; }
    public float Brake { get; set; }

    public ushort RPMRevWarning { get; set; }
    public ushort RPMRevLimiter { get; set; }

    public bool InRace { get; set; }
    public bool IsPaused { get; set; }
    public bool IsLoading { get; set; }
    public bool IsInGear { get; set; }
    public bool CarHasTurbo { get; set; }
    public bool IsRevLimiterFlashing { get; set; }
    public bool IsHandbrakeEngaged { get; set; }
    public bool IsLightsOn { get; set; }
    public bool IsLowBeamOn { get; set; }
    public bool IsHighBeamOn { get; set; }
    public bool IsASMEngaged { get; set; }
    public bool IsTCSEngaged { get; set; }

    public DateTime Timestamp { get; set; }
}

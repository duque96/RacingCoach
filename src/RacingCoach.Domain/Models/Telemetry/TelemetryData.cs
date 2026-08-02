namespace RacingCoach.Domain.Models.Telemetry;

public class TelemetryData
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public DateTime Timestamp { get; private set; }
    
    public double Speed { get; private set; }
    public double RPM { get; private set; }
    public byte Gear { get; private set; }
    public double Throttle { get; private set; }
    public double Brake { get; private set; }
    public double Steering { get; private set; }
    
    public double PositionX { get; private set; }
    public double PositionY { get; private set; }
    public double PositionZ { get; private set; }
    
    public double VelocityX { get; private set; }
    public double VelocityY { get; private set; }
    public double VelocityZ { get; private set; }
    
    public double AccelerationX { get; private set; }
    public double AccelerationY { get; private set; }
    public double AccelerationZ { get; private set; }
    
    public double TireTempFL { get; private set; }
    public double TireTempFR { get; private set; }
    public double TireTempRL { get; private set; }
    public double TireTempRR { get; private set; }
    
    public double BrakeTempFL { get; private set; }
    public double BrakeTempFR { get; private set; }
    public double BrakeTempRL { get; private set; }
    public double BrakeTempRR { get; private set; }
    
    public double SuspensionFL { get; private set; }
    public double SuspensionFR { get; private set; }
    public double SuspensionRL { get; private set; }
    public double SuspensionRR { get; private set; }
    
    public double FuelLevel { get; private set; }
    public double FuelCapacity { get; private set; }
    
    public int CurrentLap { get; private set; }
    public int TotalLaps { get; private set; }
    public int Sector { get; private set; }
    
    public TelemetryData WithSessionId(Guid sessionId)
    {
        if (sessionId == Guid.Empty)
            throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));
        
        return new TelemetryData(
            sessionId: sessionId,
            timestamp: Timestamp,
            speed: Speed,
            rpm: RPM,
            gear: Gear,
            throttle: Throttle,
            brake: Brake,
            steering: Steering,
            positionX: PositionX,
            positionY: PositionY,
            positionZ: PositionZ,
            velocityX: VelocityX,
            velocityY: VelocityY,
            velocityZ: VelocityZ,
            accelerationX: AccelerationX,
            accelerationY: AccelerationY,
            accelerationZ: AccelerationZ,
            tireTempFL: TireTempFL,
            tireTempFR: TireTempFR,
            tireTempRL: TireTempRL,
            tireTempRR: TireTempRR,
            brakeTempFL: BrakeTempFL,
            brakeTempFR: BrakeTempFR,
            brakeTempRL: BrakeTempRL,
            brakeTempRR: BrakeTempRR,
            suspensionFL: SuspensionFL,
            suspensionFR: SuspensionFR,
            suspensionRL: SuspensionRL,
            suspensionRR: SuspensionRR,
            fuelLevel: FuelLevel,
            fuelCapacity: FuelCapacity,
            currentLap: CurrentLap,
            totalLaps: TotalLaps,
            sector: Sector
        );
    }
    
    private TelemetryData() { }
    
    public TelemetryData(
        Guid sessionId,
        DateTime timestamp,
        double speed,
        double rpm,
        byte gear,
        double throttle,
        double brake,
        double steering,
        double positionX,
        double positionY,
        double positionZ,
        double velocityX,
        double velocityY,
        double velocityZ,
        double accelerationX,
        double accelerationY,
        double accelerationZ,
        double tireTempFL,
        double tireTempFR,
        double tireTempRL,
        double tireTempRR,
        double brakeTempFL,
        double brakeTempFR,
        double brakeTempRL,
        double brakeTempRR,
        double suspensionFL,
        double suspensionFR,
        double suspensionRL,
        double suspensionRR,
        double fuelLevel,
        double fuelCapacity,
        int currentLap,
        int totalLaps,
        int sector)
    {
        Id = Guid.NewGuid();
        SessionId = sessionId;
        Timestamp = timestamp;
        Speed = speed;
        RPM = rpm;
        Gear = gear;
        Throttle = throttle;
        Brake = brake;
        Steering = steering;
        PositionX = positionX;
        PositionY = positionY;
        PositionZ = positionZ;
        VelocityX = velocityX;
        VelocityY = velocityY;
        VelocityZ = velocityZ;
        AccelerationX = accelerationX;
        AccelerationY = accelerationY;
        AccelerationZ = accelerationZ;
        TireTempFL = tireTempFL;
        TireTempFR = tireTempFR;
        TireTempRL = tireTempRL;
        TireTempRR = tireTempRR;
        BrakeTempFL = brakeTempFL;
        BrakeTempFR = brakeTempFR;
        BrakeTempRL = brakeTempRL;
        BrakeTempRR = brakeTempRR;
        SuspensionFL = suspensionFL;
        SuspensionFR = suspensionFR;
        SuspensionRL = suspensionRL;
        SuspensionRR = suspensionRR;
        FuelLevel = fuelLevel;
        FuelCapacity = fuelCapacity;
        CurrentLap = currentLap;
        TotalLaps = totalLaps;
        Sector = sector;
    }
}

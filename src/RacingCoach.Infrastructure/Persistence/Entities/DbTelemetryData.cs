namespace RacingCoach.Infrastructure.Persistence.Entities;

internal class DbTelemetryData
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public DateTime Timestamp { get; set; }
    
    public double Speed { get; set; }
    public double RPM { get; set; }
    public byte Gear { get; set; }
    public double Throttle { get; set; }
    public double Brake { get; set; }
    public double Steering { get; set; }
    
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double PositionZ { get; set; }
    
    public double VelocityX { get; set; }
    public double VelocityY { get; set; }
    public double VelocityZ { get; set; }
    
    public double AccelerationX { get; set; }
    public double AccelerationY { get; set; }
    public double AccelerationZ { get; set; }
    
    public double TireTempFL { get; set; }
    public double TireTempFR { get; set; }
    public double TireTempRL { get; set; }
    public double TireTempRR { get; set; }
    
    public double BrakeTempFL { get; set; }
    public double BrakeTempFR { get; set; }
    public double BrakeTempRL { get; set; }
    public double BrakeTempRR { get; set; }
    
    public double SuspensionFL { get; set; }
    public double SuspensionFR { get; set; }
    public double SuspensionRL { get; set; }
    public double SuspensionRR { get; set; }
    
    public double FuelLevel { get; set; }
    public double FuelCapacity { get; set; }
    
    public int CurrentLap { get; set; }
    public int TotalLaps { get; set; }
    public int Sector { get; set; }
}

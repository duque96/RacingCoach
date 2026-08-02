using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RacingCoach.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Speed = table.Column<double>(type: "REAL", nullable: false),
                    RPM = table.Column<double>(type: "REAL", nullable: false),
                    Gear = table.Column<byte>(type: "INTEGER", nullable: false),
                    Throttle = table.Column<double>(type: "REAL", nullable: false),
                    Brake = table.Column<double>(type: "REAL", nullable: false),
                    Steering = table.Column<double>(type: "REAL", nullable: false),
                    PositionX = table.Column<double>(type: "REAL", nullable: false),
                    PositionY = table.Column<double>(type: "REAL", nullable: false),
                    PositionZ = table.Column<double>(type: "REAL", nullable: false),
                    VelocityX = table.Column<double>(type: "REAL", nullable: false),
                    VelocityY = table.Column<double>(type: "REAL", nullable: false),
                    VelocityZ = table.Column<double>(type: "REAL", nullable: false),
                    AccelerationX = table.Column<double>(type: "REAL", nullable: false),
                    AccelerationY = table.Column<double>(type: "REAL", nullable: false),
                    AccelerationZ = table.Column<double>(type: "REAL", nullable: false),
                    TireTempFL = table.Column<double>(type: "REAL", nullable: false),
                    TireTempFR = table.Column<double>(type: "REAL", nullable: false),
                    TireTempRL = table.Column<double>(type: "REAL", nullable: false),
                    TireTempRR = table.Column<double>(type: "REAL", nullable: false),
                    BrakeTempFL = table.Column<double>(type: "REAL", nullable: false),
                    BrakeTempFR = table.Column<double>(type: "REAL", nullable: false),
                    BrakeTempRL = table.Column<double>(type: "REAL", nullable: false),
                    BrakeTempRR = table.Column<double>(type: "REAL", nullable: false),
                    SuspensionFL = table.Column<double>(type: "REAL", nullable: false),
                    SuspensionFR = table.Column<double>(type: "REAL", nullable: false),
                    SuspensionRL = table.Column<double>(type: "REAL", nullable: false),
                    SuspensionRR = table.Column<double>(type: "REAL", nullable: false),
                    FuelLevel = table.Column<double>(type: "REAL", nullable: false),
                    FuelCapacity = table.Column<double>(type: "REAL", nullable: false),
                    CurrentLap = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalLaps = table.Column<int>(type: "INTEGER", nullable: false),
                    Sector = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_StartTime",
                table: "GameSessions",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_EndTime",
                table: "GameSessions",
                column: "EndTime");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryData_SessionId",
                table: "TelemetryData",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryData_Timestamp",
                table: "TelemetryData",
                column: "Timestamp");

            migrationBuilder.CreateTable(
                name: "ProviderConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProviderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SettingsJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderConfigurations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderConfigurations_ProviderId",
                table: "ProviderConfigurations",
                column: "ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSessions");

            migrationBuilder.DropTable(
                name: "ProviderConfigurations");

            migrationBuilder.DropTable(
                name: "TelemetryData");
        }
    }
}

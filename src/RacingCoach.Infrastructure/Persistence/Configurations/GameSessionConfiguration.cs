using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RacingCoach.Domain.Models.Sessions;

namespace RacingCoach.Infrastructure.Persistence.Configurations;

internal class GameSessionConfiguration : IEntityTypeConfiguration<GameSession>
{
    public void Configure(EntityTypeBuilder<GameSession> builder)
    {
        builder.ToTable("GameSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.GameName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.HasIndex(s => s.StartTime);
        builder.HasIndex(s => s.EndTime);
    }
}

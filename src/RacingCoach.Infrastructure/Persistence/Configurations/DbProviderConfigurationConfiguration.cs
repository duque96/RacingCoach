using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RacingCoach.Infrastructure.Persistence.Entities;

namespace RacingCoach.Infrastructure.Persistence.Configurations;

internal class DbProviderConfigurationConfiguration : IEntityTypeConfiguration<DbProviderConfiguration>
{
    public void Configure(EntityTypeBuilder<DbProviderConfiguration> builder)
    {
        builder.ToTable("ProviderConfigurations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ProviderId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.SettingsJson)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasIndex(e => e.ProviderId);
    }
}

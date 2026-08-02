using RacingCoach.Domain.Common;
using RacingCoach.Domain.Models.Providers;

namespace RacingCoach.Domain.Interfaces;

public interface IProviderConfigurationRepository
{
    Task<Result<ProviderConfiguration>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProviderConfiguration>>> GetByProviderIdAsync(string providerId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProviderConfiguration>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<Guid>> AddAsync(ProviderConfiguration config, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(ProviderConfiguration config, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

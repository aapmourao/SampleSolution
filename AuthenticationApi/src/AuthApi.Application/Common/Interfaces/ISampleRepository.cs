using AuthApi.Domain.SampleAggregate;

namespace AuthApi.Application.Common.Interfaces;

public interface ISampleRepository
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);

    Task<Sample?> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<Sample?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Sample> AddAsync(Sample sample, CancellationToken cancellationToken);

    Task<Sample> UpdateAsync(Sample sample, CancellationToken cancellationToken);

    Task DeleteAsync(Sample sample, CancellationToken cancellationToken);

    Task<List<Sample>> GetAsync(CancellationToken cancellationToken);
}

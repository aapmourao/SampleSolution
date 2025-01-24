using SharedKernel.Infrastructure.EventualConsistency;

namespace SharedKernel.Infrastructure.Persistence.Repositories;

public interface IEventualConsistencyMessageRepository
{
    Task<EventualConsistencyMessage> GetByIdAsync(Guid Id, CancellationToken cancellationToken);

    void Update(EventualConsistencyMessage eventualConsistencyMessage);

    void Remove(EventualConsistencyMessage eventualConsistencyMessage);

    Task RemoveHistoryAsync(int days, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
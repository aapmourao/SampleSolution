using SharedKernel.Infrastructure.IntegrationEvents;

namespace SharedKernel.Infrastructure.Persistence.Repositories;

public interface IInboxIntegrationEventRepository
{
    Task<List<InboxIntegrationEvent>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(InboxIntegrationEvent inboxIntegrationEvents, CancellationToken cancellationToken);
    void Update(InboxIntegrationEvent inboxIntegrationEvents);
    Task RemoveRangeAsync(List<InboxIntegrationEvent> inboxIntegrationEvents, CancellationToken cancellationToken);
    Task RemoveHistoryAsync(int days, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
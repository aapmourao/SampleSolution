using SharedKernel.Infrastructure.IntegrationEvents;

namespace SharedKernel.Infrastructure.Persistence.Repositories;

public interface IOutboxIntegrationEventRepository
{
    Task<List<OutboxIntegrationEvent>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<List<OutboxIntegrationEvent>> GetNotProcessedAsync(int batchSize, CancellationToken cancellationToken);
    Task AddAsync(OutboxIntegrationEvent outboxIntegrationEvent, CancellationToken cancellationToken);

    void Update(OutboxIntegrationEvent outboxIntegrationEvent);

    void UpdateRange(List<OutboxIntegrationEvent> outboxIntegrationEvents);

    Task RemoveRangeAsync(List<OutboxIntegrationEvent> outboxIntegrationEvents, CancellationToken cancellationToken);

    Task RemoveHistoryAsync(int days, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
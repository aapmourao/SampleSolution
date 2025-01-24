using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.IntegrationEvents;

namespace SharedKernel.Infrastructure.Persistence.Repositories;

public class OutboxIntegrationEventRepository(IOutboxDbContext dbContext, IProviderSqlAdapter providerSqlAdapter) : IOutboxIntegrationEventRepository
{
    public async Task<List<OutboxIntegrationEvent>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await dbContext.OutboxIntegrationEvents
            .OrderBy(x => x.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<OutboxIntegrationEvent>> GetNotProcessedAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await dbContext.OutboxIntegrationEvents
            .Where(x => x.ProcessedAtUtc == null)
            .OrderBy(x => x.CreatedAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(OutboxIntegrationEvent outboxIntegrationEvent, CancellationToken cancellationToken = default)
    {
        await dbContext.OutboxIntegrationEvents.AddAsync(outboxIntegrationEvent, cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Update(OutboxIntegrationEvent outboxIntegrationEvent)
    {
        dbContext.OutboxIntegrationEvents.Update(outboxIntegrationEvent);
    }
    public void UpdateRange(List<OutboxIntegrationEvent> outboxIntegrationEvents)
    {
        dbContext.OutboxIntegrationEvents.UpdateRange(outboxIntegrationEvents);
    }

    public async Task RemoveRangeAsync(List<OutboxIntegrationEvent> outboxIntegrationEvents, CancellationToken cancellationToken = default)
    {
        dbContext.OutboxIntegrationEvents.RemoveRange(outboxIntegrationEvents);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveHistoryAsync(int days, CancellationToken cancellationToken = default)
    {
        var sqlCommand = providerSqlAdapter.GetAddDaysToCurrentDateTime(
            $"DELETE FROM \"OutboxIntegrationEvents\" WHERE \"CreatedAtUtc\" < #sqlToken#", 
            days);
        _ = await dbContext.Database.ExecuteSqlAsync(FormattableStringFactory.Create(sqlCommand), cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.IntegrationEvents;
using SharedKernel.Infrastructure.Persistence;
using SharedKernel.Infrastructure.Persistence.Repositories;

namespace SharedKernel.Infrastructure.Persistence.Repositories;

public class InboxIntegrationEventRepository(IInboxDbContext dbContext, IProviderSqlAdapter providerSqlAdapter) : IInboxIntegrationEventRepository
{
    public Task<List<InboxIntegrationEvent>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.InboxIntegrationEvents
            .ToListAsync(cancellationToken);
    }

    public Task<List<InboxIntegrationEvent>> GetNotProcessedAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.InboxIntegrationEvents
            .Where(x => x.ProcessedAtUtc == null)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(InboxIntegrationEvent inboxIntegrationEvent, CancellationToken cancellationToken = default)
    {
        await dbContext.InboxIntegrationEvents.AddAsync(inboxIntegrationEvent, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// This method does not call SaveChangesAsync. 
    /// The caller is responsible for calling SaveChangesAsync after calling this method.
    /// Usually, this method is called before RemoveRangeAsync or RemoveHistoryAsync which will call SaveChangesAsync.
    /// </summary>
    /// <param name="inboxIntegrationEvents"></param>
    /// <param name="cancellationToken"></param>
    public void Update(InboxIntegrationEvent inboxIntegrationEvents)
    {
        dbContext.InboxIntegrationEvents.Update(inboxIntegrationEvents);
    }

    public async Task RemoveRangeAsync(List<InboxIntegrationEvent> inboxIntegrationEvents, CancellationToken cancellationToken = default)
    {
        dbContext.InboxIntegrationEvents.RemoveRange(inboxIntegrationEvents);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveHistoryAsync(int days, CancellationToken cancellationToken = default)
    {
        var sqlCommand = providerSqlAdapter.GetAddDaysToCurrentDateTime(
            $"DELETE FROM \"InboxIntegrationEvent\" WHERE \"CreatedAtUtc\" < #sqlToken#",
            days);

        _ = await dbContext.Database.ExecuteSqlAsync(FormattableStringFactory.Create(sqlCommand), cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
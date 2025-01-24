using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.EventualConsistency;
using CleanTemplate.Infrastructure.Persistence;

namespace SharedKernel.Infrastructure.Persistence.Repositories;

public class EventualConsistencyMessageRepository(InfrastructureDbContext dbContext, IProviderSqlAdapter providerSqlAdapter) : IEventualConsistencyMessageRepository
{
    public async Task<EventualConsistencyMessage> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        return await dbContext.EventualConsistencyMessages.FirstOrDefaultAsync(m => m.Id == Id, cancellationToken) ?? default!;
    }

    public void Update(EventualConsistencyMessage eventualConsistencyMessage)
    {
        dbContext.Update(eventualConsistencyMessage);
    }

    public void Remove(EventualConsistencyMessage eventualConsistencyMessage)
    {
        dbContext.Remove(eventualConsistencyMessage);
    }

    public async Task RemoveHistoryAsync(int days, CancellationToken cancellationToken)
    {
        var sqlCommand = providerSqlAdapter.GetAddDaysToCurrentDateTime(
            $"DELETE FROM \"EventualConsistencyMessages\" WHERE \"CreatedAtUtc\" < #sqlToken#", 
            days);
        await dbContext.Database.ExecuteSqlAsync(FormattableStringFactory.Create(sqlCommand), cancellationToken);           
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

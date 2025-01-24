using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.IntegrationEvents;

namespace SharedKernel.Infrastructure.Persistence.Repositories;

public interface IOutboxDbContext: IGenericDbContext
{
    DbSet<OutboxIntegrationEvent> OutboxIntegrationEvents { get; set; }
}

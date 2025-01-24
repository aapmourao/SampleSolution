using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.IntegrationEvents;

namespace SharedKernel.Infrastructure.Persistence.Repositories;

public interface IInboxDbContext: IGenericDbContext
{
    DbSet<InboxIntegrationEvent> InboxIntegrationEvents { get; set; }
}

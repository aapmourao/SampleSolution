using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.EventualConsistency;
using SharedKernel.Infrastructure.IntegrationEvents;

namespace SharedKernel.Infrastructure.Persistence.Repositories;

public interface IEventDbContext: IGenericDbContext
{
    DbSet<EventualConsistencyMessage> EventualConsistencyMessages { get; set; }
}
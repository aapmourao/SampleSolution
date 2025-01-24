using System.Text.Json;

using MediatR;
using SharedKernel.Infrastructure.Persistence.Repositories;
using SharedKernel.IntegrationEvents;

namespace SharedKernel.Infrastructure.IntegrationEvents.OutboxWriter;

public class OutboxWriterEventHandler(IOutboxIntegrationEventRepository outboxIntegrationEventRepository)
{

    protected async Task AddOutboxIntegrationEventAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await outboxIntegrationEventRepository.AddAsync(new OutboxIntegrationEvent(
            EventName: integrationEvent.GetType().Name,
            EventContent: JsonSerializer.Serialize(integrationEvent),
            Guid.NewGuid()),
            cancellationToken);
    }
}

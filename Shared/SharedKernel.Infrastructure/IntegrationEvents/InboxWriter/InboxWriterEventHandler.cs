using MediatR;
using SharedKernel.Infrastructure.IntegrationEvents;
using SharedKernel.Infrastructure.Persistence.Repositories;

namespace SharedKernel.Infrastructure.IntegrationEvents.OutboxWriter;

public class SubscriptionInboxWriterEventHandler(
    IInboxIntegrationEventRepository inboxIntegrationEventRepository) 
    : INotificationHandler<InboxIntegrationEvent>
{
    public async Task Handle(InboxIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await inboxIntegrationEventRepository.AddAsync(notification, cancellationToken);
    }
}
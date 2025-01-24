using SharedKernel.IntegrationEvents;

namespace SharedKernel.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;

public interface IIntegrationEventsPublisher
{
    public void PublishEvent(IIntegrationEvent integrationEvent, string correlationId);
}
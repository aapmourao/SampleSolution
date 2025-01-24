using SharedKernel.Infrastructure.Common.Model;

namespace SharedKernel.Infrastructure.IntegrationEvents;

public record OutboxIntegrationEvent(
    string EventName,
    string EventContent, 
    Guid CorrelationId = default) : TrackingProcessedStatus {}
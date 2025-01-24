namespace SharedKernel.IntegrationEvents.SubscriptionManagement;

public record PlaceRemovedIntegrationEvent(Guid PlaceId, Guid CorrelationId = default) : IIntegrationEvent;
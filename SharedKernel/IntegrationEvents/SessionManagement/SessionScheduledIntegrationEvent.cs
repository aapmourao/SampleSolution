namespace SharedKernel.IntegrationEvents.SessionManagement;

public record SessionScheduledIntegrationEvent(Guid PlaceId, Guid ProviderId) : IIntegrationEvent;
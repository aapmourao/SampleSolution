namespace SharedKernel.IntegrationEvents.SubscriptionManagement;

public record PlaceAddedIntegrationEvent(
    string Name,
    Guid PlaceId,
    Guid LocationId,
    int MaxDailySessions,
    string Metadata) : IIntegrationEvent;

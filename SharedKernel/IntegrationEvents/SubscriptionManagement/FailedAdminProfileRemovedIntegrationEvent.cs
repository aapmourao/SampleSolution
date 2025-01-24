namespace SharedKernel.IntegrationEvents.SubscriptionManagement;

public record FailedAdminProfileRemovedIntegrationEvent(Guid UserId, Guid AdminId) : IIntegrationEvent;
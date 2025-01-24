namespace SharedKernel.IntegrationEvents.UserManagement;

public record ProviderProfileRemovedIntegrationEvent(Guid UserId, Guid SubscriptionAdminId) : IIntegrationEvent;